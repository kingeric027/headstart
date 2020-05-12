using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateRequestsMapper
    {
        // takes an order worksheet, groups up the line items and returns a list of rate requests
        public static List<ShipmentEstimateRequest> Map(List<LineItem> lineItems)
        {
            var proposedShipmentGroupings = lineItems.GroupBy(li => li.ShipFromAddressID);
            return proposedShipmentGroupings.Select(proposedShipmentGrouping =>
            {
                return ShipmentEstimateRequestMapper.Map(proposedShipmentGrouping.ToList());
            }).ToList();
        }
    }
}