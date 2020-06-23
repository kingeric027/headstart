using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Common.Services;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using ordercloud.integrations.exchangerates;
using Marketplace.Models.Extended;

namespace Marketplace.Common.Commands
{
    public interface IOrderCommand
    {
        Task<Order> AcknowledgeQuoteOrder(string orderID);
        Task<ListPage<Order>> ListOrdersForLocation(string locationID, ListArgs<MarketplaceOrder> listArgs, VerifiedUserContext verifiedUser);
        Task<OrderDetails> GetOrderDetails(string orderID, VerifiedUserContext verifiedUser);
        Task<List<MarketplaceShipmentWithItems>> ListMarketplaceShipmentWithItems(string orderID, VerifiedUserContext verifiedUser);
        Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem li, VerifiedUserContext verifiedUser);
        Task RequestReturnEmail(string OrderID);
        Task SetOrderStatus(string orderID, string type);
    }

    public class OrderCommand : IOrderCommand
    {
        private readonly IOrderCloudClient _oc;
		private readonly IExchangeRatesCommand _exchangeRates;
        private readonly ISendgridService _sendgridService;
        private readonly ILocationPermissionCommand _locationPermissionCommand;
        
        public OrderCommand(IExchangeRatesCommand exchangeRates, ILocationPermissionCommand locationPermissionCommand, ISendgridService sendgridService, IOrderCloudClient oc)
        {
			_oc = oc;
            _sendgridService = sendgridService;
            _locationPermissionCommand = locationPermissionCommand;
			_exchangeRates = exchangeRates;

		}

        public async Task<Order> AcknowledgeQuoteOrder(string orderID)
        {
            int index = orderID.IndexOf("-");
            string buyerOrderID = orderID.Substring(0, index);
            await _oc.Orders.CompleteAsync(OrderDirection.Incoming, buyerOrderID);
            return await _oc.Orders.CompleteAsync(OrderDirection.Outgoing, orderID);
        }

        public async Task RequestReturnEmail(string orderID)
        {
            await _sendgridService.SendReturnRequestedEmail(orderID);
        }

        public async Task SetOrderStatus(string orderID, string type)
        {
            if (type == "cancel")
            {
               await PatchStatuses(orderID, ShippingStatus.Canceled, ClaimStatus.NoClaim, LineItemStatus.Canceled);
            }
            if (type == "requiresapproval")
            {
               await PatchStatuses(orderID, ShippingStatus.Processing, ClaimStatus.NoClaim, LineItemStatus.Submitted);
            }
            if (type == "ordershipped")
            {
               await PatchStatuses(orderID, ShippingStatus.Shipped, ClaimStatus.NoClaim, LineItemStatus.Complete);
            }
        }

        private async Task PatchStatuses(string orderID, ShippingStatus shippingStatus, ClaimStatus claimStatus, LineItemStatus lineItemStatus)
        {
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
            var partialOrder = new PartialOrder { xp = new { ShippingStatus = shippingStatus, ClaimStatus = claimStatus } };
            var partialLi = new PartialLineItem { xp = new { LineItemStatus = lineItemStatus } };
            await _oc.Orders.PatchAsync(OrderDirection.Incoming, orderID, partialOrder);
            foreach (var li in lineItems.Items)
            {
                await _oc.LineItems.PatchAsync(OrderDirection.Incoming, orderID, li.ID, partialLi);
            }
        }

        public async Task<ListPage<Order>> ListOrdersForLocation(string locationID, ListArgs<MarketplaceOrder> listArgs, VerifiedUserContext verifiedUser)
        {
            await EnsureUserCanAccessLocationOrders(locationID, verifiedUser);
            if(listArgs.Filters == null)
            {
                listArgs.Filters = new List<ListFilter>() { };
            }
            listArgs.Filters.Add(new ListFilter()
            {
                QueryParams = new List<Tuple<string, string>>() { new Tuple<string, string>("BillingAddress.ID", locationID) }
            });
            return await _oc.Orders.ListAsync(OrderDirection.Incoming,
                page: listArgs.Page,
                pageSize: listArgs.PageSize,
                search: listArgs.Search,
                filters: listArgs.ToFilterString());
        }

        public async Task<OrderDetails> GetOrderDetails(string orderID, VerifiedUserContext verifiedUser)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
            await EnsureUserCanAccessOrder(order, verifiedUser);

            // todo support >100 
            var lineItems =  _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID, pageSize: 100);
            var promotions = _oc.Orders.ListPromotionsAsync(OrderDirection.Incoming, orderID, pageSize: 100);
            var payments = _oc.Payments.ListAsync(OrderDirection.Incoming, order.ID, pageSize: 100);
            var approvals = _oc.Orders.ListApprovalsAsync(OrderDirection.Incoming, orderID, pageSize: 100);
			return new OrderDetails
            {
                Order = order,
                LineItems = (await lineItems).Items,
                Promotions = (await promotions).Items,
                Payments = (await payments).Items,
                Approvals = (await approvals).Items
			};
        }

        public async Task<List<MarketplaceShipmentWithItems>> ListMarketplaceShipmentWithItems(string orderID, VerifiedUserContext verifiedUser)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
            await EnsureUserCanAccessOrder(order, verifiedUser);

            // todo support >100 and figure out how to make these calls in parallel and 
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
            var shipments = await _oc.Orders.ListShipmentsAsync<MarketplaceShipmentWithItems>(OrderDirection.Incoming, orderID);
            var shipmentsWithItems = await Throttler.RunAsync(shipments.Items, 100, 5, (MarketplaceShipmentWithItems shipment) => GetShipmentWithItems(shipment, lineItems.Items.ToList()));
            return shipmentsWithItems.ToList();
        }

        private async Task<MarketplaceShipmentWithItems> GetShipmentWithItems(MarketplaceShipmentWithItems shipment, List<LineItem> lineItems)
        {
            var shipmentItems = await _oc.Shipments.ListItemsAsync<MarketplaceShipmentItemWithLineItem>(shipment.ID);
            shipment.ShipmentItems = shipmentItems.Items.Select(shipmentItem =>
            {
                shipmentItem.LineItem = lineItems.First(li => li.ID == shipmentItem.LineItemID);
                return shipmentItem;
            }).ToList();
            return shipment;
        }

        public async Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem liReq, VerifiedUserContext user)
        {
            var existingLineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, orderID, null, user.AccessToken);
            // New a line item
            var li = new MarketplaceLineItem();
            // If line item exists, update quantity, else create
            var preExistingLi = ((List<MarketplaceLineItem>)existingLineItems.Items).Find(eli => LineItemsMatch(eli, liReq));
            if (preExistingLi != null)
            {
                // Delete the Li
                await _oc.LineItems.DeleteAsync(OrderDirection.Outgoing, orderID, preExistingLi.ID, user.AccessToken);
            }
            // Create the new Li
            li = await _oc.LineItems.CreateAsync<MarketplaceLineItem>(OrderDirection.Outgoing, orderID, liReq, user.AccessToken);
            // Get the order, for the order.xp.Currency value
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Outgoing, orderID, user.AccessToken);
            // Exchange the li.UnitPrice from li.Product.xp.Currency => li.xp.OrderCurrency
            var exchangedUnitPrice = await ExchangeUnitPrice(li, order);
            // PATCH the Line Item with exchanged Unit Price & add ProductUnitPrice to xp (OrderDirection.Incoming due to use of elevated token)
            li = await _oc.LineItems
                .PatchAsync<MarketplaceLineItem>
                (OrderDirection.Incoming, orderID, li.ID, 
                new PartialLineItem { UnitPrice = exchangedUnitPrice, xp = new LineItemXp { UnitPriceInProductCurrency = li.UnitPrice, LineItemImageUrl = li.xp.LineItemImageUrl } });
            return li;
        }

        private bool LineItemsMatch(LineItem li1, LineItem li2)
        {
            if (li1.ProductID != li2.ProductID) return false;
            foreach (var spec1 in li1.Specs) {
                var spec2 = (li2.Specs as List<LineItemSpec>)?.Find(s => s.SpecID == spec1.SpecID);
                if (spec1?.Value != spec2?.Value) return false;
            }
            return true;
        }

        private async Task<decimal?> ExchangeUnitPrice(MarketplaceLineItem li, MarketplaceOrder order)
        {
			var supplierCurrency = li.Product?.xp?.Currency ?? CurrencySymbol.USD; // Temporary default to work around bad data.
			var buyerCurrency = order.xp.Currency ?? CurrencySymbol.USD;
			return (decimal) await _exchangeRates.ConvertCurrency(supplierCurrency, buyerCurrency, (double)li.UnitPrice);
        }

        private async Task EnsureUserCanAccessLocationOrders(string locationID, VerifiedUserContext verifiedUser, string overrideErrorMessage = "")
        {
            var hasAccess = await _locationPermissionCommand.IsUserInAccessGroup(locationID, UserGroupSuffix.ViewAllOrders.ToString(), verifiedUser);
            Require.That(hasAccess, new ErrorCode("Insufficient Access", 403, $"User cannot access orders from this location: {locationID}"));
        }

        private async Task EnsureUserCanAccessOrder(MarketplaceOrder order, VerifiedUserContext verifiedUser)
        {
            /* ensures user has access to order through at least 1 of 3 methods
             * 1) user submitted the order
             * 2) user has access to all orders from the location of the billingAddressID 
             * 3) the order is awaiting approval and the user is in the approving group 
             */ 

            var isOrderSubmitter = order.FromUserID == verifiedUser.UserID;
            if (isOrderSubmitter)
            {
                return;
            }
            
            var isUserInLocationOrderAccessGroup = await _locationPermissionCommand.IsUserInAccessGroup(order.BillingAddressID, UserGroupSuffix.ViewAllOrders.ToString(), verifiedUser);
            if (isUserInLocationOrderAccessGroup)
            {
                return;
            } 
            
            if(order.Status == OrderStatus.AwaitingApproval)
            {
                // logic assumes there is only one approving group per location
                var isUserInApprovalGroup = await _locationPermissionCommand.IsUserInAccessGroup(order.BillingAddressID, UserGroupSuffix.OrderApprover.ToString(), verifiedUser);
                if(isUserInApprovalGroup)
                {
                    return;
                }
            }

            // if function has not been exited yet we throw an insufficient access error
            Require.That(false, new ErrorCode("Insufficient Access", 403, $"User cannot access order {order.ID}"));
        }
    };
}