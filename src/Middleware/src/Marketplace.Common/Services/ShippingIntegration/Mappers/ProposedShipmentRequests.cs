using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ProposedShipmentRequestsMapper
    {
        // takes a super order, groups up the line items and returns a list of rate requests
        public static List<ProposedShipmentRequest> Map(SuperOrder obj)
        {
            var proposedShipmentGroupings = obj.LineItems.GroupBy(li => li.ShipFromAddressID);
            return proposedShipmentGroupings.Select(proposedShipmentGrouping =>
            {
                var lineItems = proposedShipmentGrouping.ToList();
                return ProposedShipmentRequestMapper.Map(lineItems);
            }).ToList();
        }
    }
}