using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using Marketplace.Common.Services.AvaTax;
using Marketplace.Common.Services;

namespace Marketplace.Common.Commands
{
    public interface IOrderCommand
    {
        Task HandleBuyerOrderSubmit(string orderId);
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

        public async Task HandleBuyerOrderSubmit(string orderId)
        {
            // should as much order submit logic to use the order calculation model as makes sense
            // so that we do not need to go get the line items and shipping information multiple times
            var buyerOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderId);
<<<<<<< refs/remotes/origin/dev
<<<<<<< refs/remotes/origin/dev
            await _taxCommand.HandleTransactionCreation(buyerOrder);
            var zoho_salesorder = await _zoho.CreateSalesOrder(buyerOrder);
=======
            //await _taxCommand.HandleTransactionCreation(buyerOrder);
=======
            var buyerOrderCalculation = await _ocSandboxService.GetOrderCalculation(OrderDirection.Incoming, orderId);
            await _avatax.CreateTransactionAsync(buyerOrderCalculation);
>>>>>>> remove tax command and use order calculation model and oc sandbox servic
            await _zoho.CreateSalesOrder(buyerOrder);
>>>>>>> order calculate in shipping integration wip
            
            var orderSplitResult = await _oc.Orders.ForwardAsync(OrderDirection.Incoming, orderId);
            var supplierOrders = orderSplitResult.OutgoingOrders;
            await ImportSupplierOrdersIntoFreightPop(supplierOrders);
            // do other order submit actions here
            await _zoho.CreatePurchaseOrder(zoho_salesorder, orderSplitResult);
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
