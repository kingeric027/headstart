using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class OrderRequestMapper
    {
        public static List<OrderRequest> Map(Order order, IList<LineItem> lineItems, Supplier supplier, Address supplierAddress)
        {
            var firstLineItem = lineItems[0];

            var orderRequest = new OrderRequest
            {
                OrderNumber = order.ID,

                // todo get carrier on order
                //Carrier = 
                Items = lineItems.Select(lineItem => ShipmentItemMapper.Map(lineItem)).ToList(),

                // add lineitems should have the same ship to ship from when passed into this function
                ShipperAddress = OrderAddressMapper.Map(firstLineItem.ShipFromAddress),
                ConsigneeAddress = OrderAddressMapper.Map(firstLineItem.ShippingAddress),

                // hardcoding this as sales order for now, all orders going into freightpop should be
                // sales orders to the supplier whose account the order will go into
                TransactionType = OrderTransactionType.Sales,

                // We may want to use Reference 1 as the api client ID so that the order can flow back into
                // ordercloud into the right orgs 
                // Reference1

                // what is payment term for?
                PaymentTerm = PaymentTerm.Sender,

                // will need to figure out what freightpop is looking for with this information
                ThirdPartyAccountInfo = AccountDetailsMapper.Map(supplier, supplierAddress)
            };

            return new List<OrderRequest>
            {
                orderRequest
            };
        }
    }
}