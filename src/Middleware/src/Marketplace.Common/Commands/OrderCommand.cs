using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderCloud.SDK;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using Marketplace.Models.Extended;
using Marketplace.Common.Services;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.extensions;
using Marketplace.Common.Services.ShippingIntegration;
using ordercloud.integrations.avalara;
using Marketplace.Models.Misc;
using ordercloud.integrations.exchangerates;
using Flurl.Http;

namespace Marketplace.Common.Commands
{
    public interface IOrderCommand
    {
        Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet order);
        Task<Order> AcknowledgeQuoteOrder(string orderID);
        Task<ListPage<Order>> ListOrdersForLocation(string locationID, ListArgs<MarketplaceOrder> listArgs, VerifiedUserContext verifiedUser);
        Task<OrderDetails> GetOrderDetails(string orderID, VerifiedUserContext verifiedUser);
        Task<List<MarketplaceShipmentWithItems>> GetMarketplaceShipmentWithItems(string orderID, VerifiedUserContext verifiedUser);
        Task<MarketplaceLineItem> UpsertLineItem(string orderID, MarketplaceLineItem li, VerifiedUserContext verifiedUser);
    }

    public class OrderCommand : IOrderCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;
        private readonly string OcIntegrationsApiBaseUrl = "https://ordercloud-middleware-test.azurewebsites.net";

        // temporary service until we get updated sdk
        private readonly IOrderCloudSandboxService _ocSandboxService;
        private readonly IZohoCommand _zoho;
        private readonly IAvalaraCommand _avalara;
        private readonly ISendgridService _sendgridService;
        private readonly ILocationPermissionCommand _locationPermissionCommand;
        
        public OrderCommand(ILocationPermissionCommand locationPermissionCommand, IFreightPopService freightPopService, ISendgridService sendgridService, IOCShippingIntegration ocShippingIntegration, IAvalaraCommand avatax, IOrderCloudClient oc, IZohoCommand zoho, IOrderCloudSandboxService orderCloudSandboxService)
        {
            _freightPopService = freightPopService;
			_oc = oc;
            _ocShippingIntegration = ocShippingIntegration;
            _avalara = avatax;
            _zoho = zoho;
            _sendgridService = sendgridService;
            _ocSandboxService = orderCloudSandboxService;
            _locationPermissionCommand = locationPermissionCommand;
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

            // todo support >100 and figure out how to make these calls in parallel and 
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
                new PartialLineItem { UnitPrice = exchangedUnitPrice, xp = new LineItemXp { ProductUnitPrice = li.UnitPrice} });
            return li;
        }

        private bool LineItemsMatch(LineItem li1, LineItem li2)
        {
            if (li1.ProductID != li2.ProductID) return false;
            if (li1.Specs.Count == 0 || li2.Specs.Count == 0) return false;
            foreach (var spec1 in li1.Specs) {
                var spec2 = (li2.Specs as List<LineItemSpec>)?.Find(s => s.SpecID == spec1.SpecID);
                if (spec1.Value != spec2.Value) return false;
            }
            return true;
        }

        private async Task<decimal?> ExchangeUnitPrice(MarketplaceLineItem li, MarketplaceOrder order)
        {
            // Exchange the li.UnitPrice from li.Product.xp.Currency => li.xp.OrderCurrency
            var url = OcIntegrationsApiBaseUrl + $"/exchangerates/{li.Product.xp.Currency}";
            var ExchangeRates = await url.GetJsonAsync<ListPage<OrderCloudIntegrationsConversionRate>>();
            CurrencySymbols liCurrency = (CurrencySymbols)Enum.Parse(typeof(CurrencySymbols), order.xp.Currency);
            var OrderRate = (ExchangeRates.Items as List<OrderCloudIntegrationsConversionRate>).Find(r => r.Currency == liCurrency);
            return (decimal)li.UnitPrice * (decimal)OrderRate.Rate;
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
            var transaction = await _avalara.CreateTransactionAsync(orderWorksheet);
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