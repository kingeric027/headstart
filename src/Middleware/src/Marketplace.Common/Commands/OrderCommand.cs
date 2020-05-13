using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using Marketplace.Models.Extended;
using Marketplace.Common.Services;
using Marketplace.Common.Services.Avalara;
using Marketplace.Common.Services.ShippingIntegration.Models;
using System.Linq;
using Marketplace.Models.Models.Marketplace;
using System;
using Newtonsoft.Json;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;

namespace Marketplace.Common.Commands
{
    public interface IOrderCommand
    {
        Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet order);
        Task<Order> AcknowledgeQuoteOrder(string orderID);
        Task<ListPage<Order>> ListOrdersForLocation(string locationID, ListArgs<MarketplaceOrder> listArgs, VerifiedUserContext verifiedUser);
        Task<OrderDetails> GetOrderDetails(string orderID, VerifiedUserContext verifiedUser);
        Task<List<MarketplaceShipmentWithItems>> GetMarketplaceShipmentWithItems(string orderID, VerifiedUserContext verifiedUser);
    }

    public class OrderCommand : IOrderCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;

        // temporary service until we get updated sdk
        private readonly IOrderCloudSandboxService _ocSandboxService;
        private readonly IZohoCommand _zoho;
        private readonly IAvalaraCommand _avatax;
        private readonly ISendgridService _sendgridService;
        
        public OrderCommand(IFreightPopService freightPopService, ISendgridService sendgridService, IOCShippingIntegration ocShippingIntegration, IAvalaraCommand avatax, IOrderCloudClient oc, IZohoCommand zoho, IOrderCloudSandboxService orderCloudSandboxService)
        {
            _freightPopService = freightPopService;
			_oc = oc;
            _ocShippingIntegration = ocShippingIntegration;
            _avatax = avatax;
            _zoho = zoho;
            _sendgridService = sendgridService;
            _ocSandboxService = orderCloudSandboxService;
        }

        public async Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet orderWorksheet)
        {
            try
            {
                // forwarding
                var buyerOrder = orderWorksheet.Order;
                await CleanIDLineItems(orderWorksheet);

                var orderSplitResult = await _oc.Orders.ForwardAsync(OrderDirection.Incoming, buyerOrder.ID);
                var supplierOrders = orderSplitResult.OutgoingOrders.ToList();

                // creating relationship between the buyer order and the supplier order
                // no relationship exists currently in the platform
                var updatedSupplierOrders = await CreateOrderRelationshipsAndTransferXP(buyerOrder, supplierOrders);
                
                // leaving this in until the sdk supports type parameters on order worksheet
                var updatedWorksheet = await _ocSandboxService.GetOrderWorksheetAsync(OrderDirection.Incoming, buyerOrder.ID);
                
                await _sendgridService.SendOrderSubmitEmail(orderWorksheet);
                
                // quote orders do not need to flow into our integrations
                if (buyerOrder.xp == null || buyerOrder.xp.OrderType != OrderType.Quote)
                {
                    await ImportSupplierOrdersIntoFreightPop(updatedSupplierOrders);
                    await HandleTaxTransactionCreationAsync(orderWorksheet);
                    var zoho_salesorder = await _zoho.CreateSalesOrder(orderWorksheet);
                    await _zoho.CreatePurchaseOrder(zoho_salesorder, orderSplitResult);
                }

                var response = new OrderSubmitResponse()
                {
                    HttpStatusCode = 200,
                };
                return response;
            }
            catch (Exception ex)
            {
                var response = new OrderSubmitResponse()
                {
                    HttpStatusCode = 500,
                    UnhandledErrorBody = JsonConvert.SerializeObject(ex),
                };
                return response;
            }
        }

        public async Task<Order> AcknowledgeQuoteOrder(string orderID)
        {
            int index = orderID.IndexOf("-");
            string buyerOrderID = orderID.Substring(0, index);
            await _oc.Orders.CompleteAsync(OrderDirection.Incoming, buyerOrderID);
            return await _oc.Orders.CompleteAsync(OrderDirection.Outgoing, orderID);
        }

        public async Task<ListPage<Order>> ListOrdersForLocation(string locationID, ListArgs<MarketplaceOrder> listArgs, VerifiedUserContext verifiedUser)
        {
            await EnsureUserCanAccessLocationOrders(locationID, verifiedUser);
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
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
            var promotions = await _oc.Orders.ListPromotionsAsync(OrderDirection.Incoming, orderID);
            var payments = await _oc.Payments.ListAsync(OrderDirection.Incoming, order.ID);
            var approvals = await _oc.Orders.ListApprovalsAsync(OrderDirection.Incoming, orderID);
            return new OrderDetails
            {
                Order = order,
                LineItems = lineItems,
                Promotions = promotions,
                Payments = payments,
                Approvals = approvals
            };
        }

        public async Task<List<MarketplaceShipmentWithItems>> GetMarketplaceShipmentWithItems(string orderID, VerifiedUserContext verifiedUser)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
            await EnsureUserCanAccessOrder(order, verifiedUser);
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


        private async Task EnsureUserCanAccessLocationOrders(string locationID, VerifiedUserContext verifiedUser, string overrideErrorMessage = "")
        {
            var hasAccess = await IsUserInAccessGroup(locationID, "ViewAllLocationOrders", verifiedUser);
            Require.That(hasAccess, new ErrorCode("Insufficient Access", 403, $"User cannot access orders from this location: {locationID}"));
        }

        private async Task<bool> IsUserInAccessGroup(string locationID, string groupSuffix, VerifiedUserContext verifiedUser)
        {
            var buyerID = verifiedUser.BuyerID;
            var userGroupID = $"{locationID}-{groupSuffix}";
            return await IsUserInUserGroup(buyerID, userGroupID, verifiedUser);
        }

        private async Task<bool> IsUserInUserGroup(string buyerID, string userGroupID, VerifiedUserContext verifiedUser)
        {
            var userGroupAssignmentForAccess = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID, verifiedUser.UserID);
            return userGroupAssignmentForAccess.Items.Count > 0;
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
            
            var isUserInLocationOrderAccessGroup = await IsUserInAccessGroup(order.BillingAddressID, "ViewAllLocationOrders", verifiedUser);
            if (isUserInLocationOrderAccessGroup)
            {
                return;
            } 
            
            if(order.Status == OrderStatus.AwaitingApproval)
            {
                // logic assumes there is only one approving group per location
                var isUserInApprovalGroup = await IsUserInAccessGroup(order.BillingAddressID, "OrderApprover", verifiedUser);
                if(isUserInApprovalGroup)
                {
                    return;
                }
            }

            // if function has not been exited yet we throw an insufficient access error
            Require.That(false, new ErrorCode("Insufficient Access", 403, $"User cannot access order {order.ID}"));
        }

        private async Task CleanIDLineItems(MarketplaceOrderWorksheet orderWorksheet)
        {
            /* line item ids are significant for suppliers creating a relationship
            * between their shipments and line items in ordercloud 
            * we are sequentially labeling these ids for ease of shipping */

            var lineItemIDChanges = orderWorksheet.LineItems.Select((li, index) => (OldID: li.ID, NewID: CreateIDFromIndex(index)));
            await Throttler.RunAsync(lineItemIDChanges, 100, 2, (lineItemIDChange) =>
            {
                return _oc.LineItems.PatchAsync(OrderDirection.Incoming, orderWorksheet.Order.ID, lineItemIDChange.OldID, new PartialLineItem { ID = lineItemIDChange.NewID });
            });
        }

        private string CreateIDFromIndex(int index)
        {
            /* X was choosen as a prefix for the lineItem ID so that it is easy to 
               * direct suppliers where to look for the ID. L and I are sometimes indistinguishable 
               * from the number 1 so I avoided those. X is also difficult to confuse with other
               * letters when verbally pronounced */
            var countInList = index + 1;
            var paddedCount = countInList.ToString().PadLeft(3, '0');
            return 'X' + paddedCount;
        }

        private async Task<List<MarketplaceOrder>> CreateOrderRelationshipsAndTransferXP(MarketplaceOrder buyerOrder, List<Order> supplierOrders)
        {
            var updatedSupplierOrders = new List<MarketplaceOrder>();
            var supplierIDs = new List<string>();
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, buyerOrder.ID);
            var shipFromAddressIDs = GetShipFromAddressIDs(lineItems.Items.ToList());

            foreach (var supplierOrder in supplierOrders)
            {
                var supplierID = supplierOrder.ToCompanyID;
                supplierIDs.Add(supplierID);
                var shipFromAddressIDsForSupplierOrder = shipFromAddressIDs.Where(addressID => addressID.Contains(supplierID)).ToList();
                var supplierOrderPatch = new PartialOrder()
                {
                    ID = $"{buyerOrder.ID}-{supplierID}",
                    xp = GetNewOrderXP(buyerOrder, supplierID, shipFromAddressIDsForSupplierOrder)
                };
                var updatedSupplierOrder = await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Outgoing, supplierOrder.ID, supplierOrderPatch);
                updatedSupplierOrders.Add(updatedSupplierOrder);
            }

            var buyerOrderPatch = new PartialOrder()
            {
                xp = new
                {
                    ShipFromAddressIDs = shipFromAddressIDs,
                    SupplierIDs = supplierIDs
                }
            };
            await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrder.ID, buyerOrderPatch);
            return updatedSupplierOrders;
        }

        private OrderXp GetNewOrderXP(MarketplaceOrder buyerOrder, string supplierID, List<string> shipFromAddressIDsForSupplierOrder)
        {
            var supplierOrderXp = new OrderXp()
            {
                ShipFromAddressIDs = shipFromAddressIDsForSupplierOrder,
                SupplierIDs = new List<string>() { supplierID },
                StopShipSync = false,
                OrderType = buyerOrder.xp.OrderType,
                QuoteOrderInfo = buyerOrder.xp.QuoteOrderInfo
            };
            return supplierOrderXp;
        }

        private List<string> GetShipFromAddressIDs(List<LineItem> lineItems)
        {
            var shipFromAddressIDs = new List<string>();
            lineItems.ForEach(li => {
                if (!shipFromAddressIDs.Contains(li.ShipFromAddressID))
                {
                    shipFromAddressIDs.Add(li.ShipFromAddressID);
                }
            });
            return shipFromAddressIDs;
        }

        private async Task HandleTaxTransactionCreationAsync(MarketplaceOrderWorksheet orderWorksheet)
        {
            var transaction = await _avatax.CreateTransactionAsync(orderWorksheet);
            await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderWorksheet.Order.ID, new PartialOrder()
            {
                TaxCost = transaction.totalTax ?? 0,  // Set this again just to make sure we have the most up to date info
                xp = new { AvalaraTaxTransactionCode = transaction.code }
            });
        }

        private async Task ImportSupplierOrdersIntoFreightPop(IList<MarketplaceOrder> supplierOrders)
        {
            foreach (var supplierOrder in supplierOrders)
            {
                await ImportSupplierOrderIntoFreightPop(supplierOrder);
            }
        }

        private async Task ImportSupplierOrderIntoFreightPop(MarketplaceOrder supplierOrder)
        {

            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, supplierOrder.ID);
            var firstLineItemOfSupplierOrder = lineItems.Items.First();
            var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(firstLineItemOfSupplierOrder.SupplierID);

            if (supplier.xp.SyncFreightPop)
            {
                // we further split the supplier order into multiple orders for each shipfromaddressID before it goes into freightpop
                var freightPopOrders = lineItems.Items.GroupBy(li => li.ShipFromAddressID);

                var freightPopOrderIDs = new List<string>();
                foreach (var lineItemGrouping in freightPopOrders)
                {
                    var firstLineItem = lineItemGrouping.First();

                    var freightPopOrderID = $"{supplierOrder.ID.Split('-').First()}-{firstLineItem.ShipFromAddressID}";
                    freightPopOrderIDs.Add(freightPopOrderID);

                    var supplierAddress = await _oc.SupplierAddresses.GetAsync(supplier.ID, firstLineItem.ShipFromAddressID);
                    var freightPopOrderRequest = OrderRequestMapper.Map(supplierOrder, lineItemGrouping.ToList(), supplier, supplierAddress, freightPopOrderID);
                    await _freightPopService.ImportOrderAsync(freightPopOrderRequest);
                }
            }
        }
    };
}