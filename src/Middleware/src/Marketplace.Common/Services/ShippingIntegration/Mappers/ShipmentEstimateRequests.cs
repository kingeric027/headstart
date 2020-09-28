using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateRequestsMapper
    {
        // takes an order worksheet, groups up the line items and returns a list of rate requests
        public static List<ShipmentEstimateRequest> Map(List<LineItem> lineItems)
        {
            var proposedShipmentGroupings = GetLineItemGroupings(lineItems);
            return proposedShipmentGroupings.Select(proposedShipmentGrouping =>
            {
                return ShipmentEstimateRequestMapper.Map(proposedShipmentGrouping.ToList());
            }).ToList();
        }

        private static IEnumerable<IGrouping<string,LineItem>> GetLineItemGroupings(List<LineItem> lineItems)
        {
            var standardLineItems = lineItems.Where(li => li.Product.xp.ProductType == ProductType.Standard.ToString()).GroupBy(li => li.ShipFromAddressID);
            var purchaseOrderLineItems = lineItems.Where(li => li.Product.xp.ProductType == ProductType.PurchaseOrder.ToString()).GroupBy(li => li.ShipFromAddressID);
            return standardLineItems.Concat(purchaseOrderLineItems);
        }
    }
}   