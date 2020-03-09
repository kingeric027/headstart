using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class OCShipmentMapper
    {
        public static Shipment Map(ShipmentDetails freightPopShipment, string buyerID)
        {
            return new Shipment
            {
                Shipper = "Fedex",
                DateShipped = freightPopShipment.ShipDate,

                // when would there be more than one tracking number for a single freightpop shipment?
                TrackingNumber = freightPopShipment.TrackingNumbers.First(),
                Cost = freightPopShipment.Rate.DiscountedCost,
                xp = freightPopShipment.Rate
            };
        }
    }
}