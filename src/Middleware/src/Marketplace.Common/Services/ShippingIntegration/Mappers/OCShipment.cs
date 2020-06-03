using ordercloud.integrations.freightpop;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
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
                ID = freightPopShipment.ShipmentId,
                Shipper = "FedEx",
                DateShipped = freightPopShipment.ShipDate,
                BuyerID = buyerID,
                // when/why would there be more than one tracking number for a single freightpop shipment?
                TrackingNumber = freightPopShipment.TrackingNumbers.First(),
                Cost = freightPopShipment.Rate.DiscountedCost,
                xp = new ShipmentXp
                {
                    FreightPopShipmentRate = freightPopShipment.Rate,
                    Service = freightPopShipment.Rate.Service
                }
            };
        }
    }
}