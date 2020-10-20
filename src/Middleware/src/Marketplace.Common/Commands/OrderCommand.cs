using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Common.Services;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using ordercloud.integrations.exchangerates;
using Marketplace.Models.Extended;
using Microsoft.AspNetCore.Mvc.Filters;
using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Commands
{
    public interface IOrderCommand
    {
        Task<Order> AcknowledgeQuoteOrder(string orderID);
        Task<ListPage<MarketplaceOrder>> ListOrdersForLocation(string locationID, ListArgs<MarketplaceOrder> listArgs, VerifiedUserContext verifiedUser);
        Task<OrderDetails> GetOrderDetails(string orderID, VerifiedUserContext verifiedUser);
        Task<List<MarketplaceShipmentWithItems>> ListMarketplaceShipmentWithItems(string orderID, VerifiedUserContext verifiedUser);
        Task<MarketplaceOrder> AddPromotion(string orderID, string promoCode, VerifiedUserContext verifiedUser);
        Task<MarketplaceOrder> ApplyAutomaticPromotions(string orderID);
        Task PatchOrderRequiresApprovalStatus(string orderID);
    }

    public class OrderCommand : IOrderCommand
    {
        private readonly IOrderCloudClient _oc;
		private readonly IExchangeRatesCommand _exchangeRates;
        private readonly ISendgridService _sendgridService;
        private readonly ILocationPermissionCommand _locationPermissionCommand;
        private readonly IMeProductCommand _meProductCommand;
        private readonly IPromotionCommand _promotionCommand;
        
        public OrderCommand(IExchangeRatesCommand exchangeRates, ILocationPermissionCommand locationPermissionCommand, ISendgridService sendgridService, IOrderCloudClient oc, IMeProductCommand meProductCommand, IPromotionCommand promotionCommand)
        {
			_oc = oc;
            _sendgridService = sendgridService;
            _locationPermissionCommand = locationPermissionCommand;
			_exchangeRates = exchangeRates;
            _meProductCommand = meProductCommand;
            _promotionCommand = promotionCommand;
		}

        public async Task<Order> AcknowledgeQuoteOrder(string orderID)
        {
            var orderPatch = new PartialOrder()
            {
                xp = new
                {
                    SubmittedOrderStatus = SubmittedOrderStatus.Completed
                }
            };
            //  Need to complete sales and purchase order and patch the xp.SubmittedStatus of both orders            
            var salesOrderID = orderID.Split('-')[0];
            var completeSalesOrder = _oc.Orders.CompleteAsync(OrderDirection.Incoming, salesOrderID);
            var patchSalesOrder = _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, salesOrderID, orderPatch);
            var completedSalesOrder = await completeSalesOrder;
            var patchedSalesOrder = await patchSalesOrder;

            var purchaseOrderID = $"{salesOrderID}-{patchedSalesOrder?.xp?.SupplierIDs?.FirstOrDefault()}";
            var completePurchaseOrder = _oc.Orders.CompleteAsync(OrderDirection.Outgoing, purchaseOrderID);
            var patchPurchaseOrder = _oc.Orders.PatchAsync(OrderDirection.Outgoing, purchaseOrderID, orderPatch);
            var completedPurchaseOrder = await completePurchaseOrder;
            var patchedPurchaseOrder = await patchPurchaseOrder;

            return orderID == salesOrderID ? patchedSalesOrder : patchedPurchaseOrder;
        }
       
        public async Task PatchOrderRequiresApprovalStatus(string orderID)
        {
                await PatchOrderStatus(orderID, ShippingStatus.Processing, ClaimStatus.NoClaim);
        }

        private async Task PatchOrderStatus(string orderID, ShippingStatus shippingStatus, ClaimStatus claimStatus)
        {
            var partialOrder = new PartialOrder { xp = new { ShippingStatus = shippingStatus, ClaimStatus = claimStatus } };
            await _oc.Orders.PatchAsync(OrderDirection.Incoming, orderID, partialOrder);
        }

        public async Task<ListPage<MarketplaceOrder>> ListOrdersForLocation(string locationID, ListArgs<MarketplaceOrder> listArgs, VerifiedUserContext verifiedUser)
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
            return await _oc.Orders.ListAsync<MarketplaceOrder>(OrderDirection.Incoming,
                page: listArgs.Page,
                pageSize: listArgs.PageSize,
                search: listArgs.Search,
                sortBy: listArgs.SortBy.FirstOrDefault(),
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

        public async Task<MarketplaceOrder> AddPromotion(string orderID, string promoCode, VerifiedUserContext verifiedUser)
        {
            var orderPromo = await _oc.Orders.AddPromotionAsync(OrderDirection.Incoming, orderID, promoCode);
            return await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
        }

        public async Task<MarketplaceOrder> ApplyAutomaticPromotions(string orderID)
        {
            await _promotionCommand.AutoApplyPromotions(orderID);
            return await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
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