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

namespace Marketplace.Common.Commands
{
    public interface IOrderCommand
    {
        Task HandleBuyerOrderSubmit(Order order);
    }

    public class OrderCommand : IOrderCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;

        // temporary service until we get updated sdk
        private readonly IOrderCloudSandboxService _ocSandboxService;
        private readonly IZohoCommand _zoho;
        private readonly IAvataxService _avatax;

        public OrderCommand(IFreightPopService freightPopService, IOCShippingIntegration ocShippingIntegration, IAvataxService avatax, IOrderCloudClient oc, IZohoCommand zoho, IOrderCloudSandboxService orderCloudSandboxService)
        {
            _freightPopService = freightPopService;
			_oc = oc;
            _ocShippingIntegration = ocShippingIntegration;
            _avatax = avatax;
            _zoho = zoho;
            _ocSandboxService = orderCloudSandboxService;
        }

        public async Task HandleBuyerOrderSubmit(Order order)
        {
            // forwarding
            var orderSplitResult = await _oc.Orders.ForwardAsync(OrderDirection.Incoming, order.ID);
            var supplierOrders = orderSplitResult.OutgoingOrders;

            // creating relationship between the buyer order and the supplier order
            // no relationship exists currently in the platform
            var updatedSupplierOrders = await CreateOrderRelationships(order.ID, supplierOrders.ToList());
            
            // quote orders do not need to flow into our integrations
            if (order.xp == null || order.xp.OrderType != OrderType.Quote)
            {
                var buyerOrderWorksheet = await _ocSandboxService.GetOrderWorksheetAsync(OrderDirection.Incoming, order.ID);
                await ImportSupplierOrdersIntoFreightPop(updatedSupplierOrders);
                
                // temporarily do not do these integrations until platform bug that results in nulled fields on order worksheet is fixed
                if(buyerOrderWorksheet.ShipEstimateResponse != null) {
                    await HandleTaxTransactionCreationAsync(buyerOrderWorksheet);
                    var zoho_salesorder = await _zoho.CreateSalesOrder(buyerOrderWorksheet);
                    await _zoho.CreatePurchaseOrder(zoho_salesorder, orderSplitResult);
                }
            }
        }

        private async Task<List<Order>> CreateOrderRelationships(string buyerOrderID, List<Order> supplierOrders)
        {
            var updatedSupplierOrders = new List<Order>();
            var supplierIDs = new List<string>();
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, buyerOrderID);
            var shipFromAddressIDs = GetShipFromAddressIDs(lineItems.Items.ToList());

            foreach (var supplierOrder in supplierOrders)
            {
                var supplierID = supplierOrder.ToCompanyID;
                supplierIDs.Add(supplierID);
                var shipFromAddressIDsForSupplierOrder = shipFromAddressIDs.Where(addressID => addressID.Contains(supplierID));
                var supplierOrderPatch = new PartialOrder()
                {
                    ID = $"{buyerOrderID}-{supplierID}",
                    xp = new
                    {
                        ShipFromAddressIDs = shipFromAddressIDsForSupplierOrder,
                        SupplierIDs = new List<string>() { supplierID },
                        StopShipSync = false
                    }
                };
                var updatedSupplierOrder = await _oc.Orders.PatchAsync(OrderDirection.Outgoing, supplierOrder.ID, supplierOrderPatch);
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
            await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrderID, buyerOrderPatch);
            return updatedSupplierOrders;
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

        private async Task HandleTaxTransactionCreationAsync(OrderWorksheet orderWorksheet)
        {
            var transaction = await _avatax.CreateTransactionAsync(orderWorksheet);
            await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderWorksheet.Order.ID, new PartialOrder()
            {
                TaxCost = transaction.totalTax ?? 0,  // Set this again just to make sure we have the most up to date info
                xp = new { AvalaraTaxTransactionCode = transaction.code }
            });
        }

        private async Task ImportSupplierOrdersIntoFreightPop(IList<Order> supplierOrders)
        {
            foreach(var supplierOrder in supplierOrders)
            {
                await ImportSupplierOrderIntoFreightPop(supplierOrder);
            }
        }

        private async Task ImportSupplierOrderIntoFreightPop(Order supplierOrder)
        {

            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, supplierOrder.ID);
            
            // we further split the supplier order into multiple orders for each shipfromaddressID before it goes into freightpop
            var freightPopOrders = lineItems.Items.GroupBy(li => li.ShipFromAddressID);

            var freightPopOrderIDs = new List<string>();
            foreach(var lineItemGrouping in freightPopOrders)
            {
                var firstLineItem = lineItemGrouping.First();

                var freightPopOrderID = $"{supplierOrder.ID.Split('-').First()}-{firstLineItem.ShipFromAddressID}";
                freightPopOrderIDs.Add(freightPopOrderID);

                var supplier = await _oc.Suppliers.GetAsync(firstLineItem.SupplierID);
                var supplierAddress = await _oc.SupplierAddresses.GetAsync(supplier.ID, firstLineItem.ShipFromAddressID);
                var freightPopOrderRequest = OrderRequestMapper.Map(supplierOrder, lineItemGrouping.ToList(), supplier, supplierAddress, freightPopOrderID);
                await _freightPopService.ImportOrderAsync(freightPopOrderRequest);
            }
        }
    }
}
