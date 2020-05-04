using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using Marketplace.Models.Extended;
using Marketplace.Common.Services.AvaTax;
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
        Task<ListPage<Order>> ListOrdersForLocation(string locationID, VerifiedUserContext verifiedUser);
    }

    public class OrderCommand : IOrderCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;

        // temporary service until we get updated sdk
        private readonly IOrderCloudSandboxService _ocSandboxService;
        private readonly IZohoCommand _zoho;
        private readonly IAvalaraService _avatax;
        private readonly ISendgridService _sendgridService;
        
        public OrderCommand(IFreightPopService freightPopService, ISendgridService sendgridService, IOCShippingIntegration ocShippingIntegration, IAvalaraService avatax, IOrderCloudClient oc, IZohoCommand zoho, IOrderCloudSandboxService orderCloudSandboxService)
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

        public async Task<ListPage<Order>> ListOrdersForLocation(string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserCanAccessLocationOrders(locationID, verifiedUser);
            return await _oc.Orders.ListAsync(OrderDirection.Incoming, opts => opts.AddFilter(o => o.BillingAddress.ID == locationID));
        }

        private async Task EnsureUserCanAccessLocationOrders(string locationID, VerifiedUserContext verifiedUser)
        {
            var buyerID = verifiedUser.BuyerID;
            var userGroupID = $"{locationID}-ViewAllLocationOrders";
            var userGroupAssignmentForAccess = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID, verifiedUser.UserID);
            Require.That(userGroupAssignmentForAccess.Items.Count > 0, new ErrorCode("Insufficient Access", 403, $"User cannot access orders from this location: {locationID}"));
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
            foreach(var supplierOrder in supplierOrders)
            {
                await ImportSupplierOrderIntoFreightPop(supplierOrder);
            }
        }

        private async Task ImportSupplierOrderIntoFreightPop(MarketplaceOrder supplierOrder)
        {

            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, supplierOrder.ID);
            var firstLineItemOfSupplierOrder = lineItems.Items.First();
            var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(firstLineItemOfSupplierOrder.SupplierID);
            
            if(supplier.xp.SyncFreightPop)
            {
                // we further split the supplier order into multiple orders for each shipfromaddressID before it goes into freightpop
                var freightPopOrders = lineItems.Items.GroupBy(li => li.ShipFromAddressID);

                var freightPopOrderIDs = new List<string>();
                foreach(var lineItemGrouping in freightPopOrders)
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
    
    }
}
