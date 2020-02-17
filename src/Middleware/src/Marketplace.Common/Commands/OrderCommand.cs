using Marketplace.Common.Exceptions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;

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
        private readonly ITaxCommand _taxCommand;
        private readonly IZohoCommand _zoho;

        public OrderCommand(IFreightPopService freightPopService, IOCShippingIntegration ocShippingIntegration, ITaxCommand taxCommand, IOrderCloudClient oc, IZohoCommand zoho)
        {
            _freightPopService = freightPopService;
			_oc = oc;
            _ocShippingIntegration = ocShippingIntegration;
            _taxCommand = taxCommand;
            _zoho = zoho;
        }

        public async Task HandleBuyerOrderSubmit(string orderId)
        {
            var buyerOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderId);
            await _taxCommand.HandleTransactionCreation(buyerOrder);
            await _zoho.CreateSalesOrder(buyerOrder);
            
            var orderSplitResult = await _oc.Orders.ForwardAsync(OrderDirection.Incoming, orderId);
            var supplierOrders = orderSplitResult.OutgoingOrders;
            await ImportSupplierOrdersIntoFreightPop(supplierOrders);

            // do other order submit actions here
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
