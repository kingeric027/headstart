using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateRequestsMapper
    {
        // takes an order worksheet, groups up the line items and returns a list of rate requests
        public static List<ShipmentEstimateRequest> Map(OrderWorksheet obj)
        {
            var proposedShipmentGroupings = obj.LineItems.GroupBy(li => li.ShipFromAddressID);
            return proposedShipmentGroupings.Select(proposedShipmentGrouping =>
            {
                var lineItems = proposedShipmentGrouping.ToList();
                return ShipmentEstimateRequestMapper.Map(lineItems);
            }).ToList();
        }
    }
}