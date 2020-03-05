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
using Marketplace.Common.Services.ShippingIntegration.Models;

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
            if (order.xp.OrderType == OrderType.Quote)
            {
                await _oc.Orders.ForwardAsync(OrderDirection.Incoming, order.ID);
            }
            else
            {
            // forwarding
            var buyerOrderCalculation = await _ocSandboxService.GetOrderCalculation(OrderDirection.Incoming, orderId);
            var orderSplitResult = await _oc.Orders.ForwardAsync(OrderDirection.Incoming, orderId);
            var supplierOrders = orderSplitResult.OutgoingOrders;
            
            // integrations
            var zoho_salesorder = await _zoho.CreateSalesOrder(buyerOrderCalculation);
            await HandleTaxTransactionCreationAsync(buyerOrderCalculation);
            await ImportSupplierOrdersIntoFreightPop(supplierOrders);
            await _zoho.CreatePurchaseOrder(zoho_salesorder, orderSplitResult);
        }

        private async Task HandleTaxTransactionCreationAsync(OrderCalculation orderCalculation)
        {
            var transaction = await _avatax.CreateTransactionAsync(orderCalculation);
            await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderCalculation.Order.ID, new PartialOrder()
            {
                TaxCost = transaction.totalTax ?? 0,  // Set this again just to make sure we have the most up to date info
                xp = { AvalaraTaxTransactionCode = transaction.code }
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
            var firstLineItem = lineItems.Items[0];
            var supplier = await _oc.Suppliers.GetAsync(firstLineItem.SupplierID);
            var supplierAddress = await _oc.SupplierAddresses.GetAsync(supplier.ID, firstLineItem.ShipFromAddressID);
            var freightPopOrderRequest = OrderRequestMapper.Map(supplierOrder, lineItems.Items, supplier, supplierAddress);
            await _freightPopService.ImportOrderAsync(freightPopOrderRequest);
        }
    }
}
