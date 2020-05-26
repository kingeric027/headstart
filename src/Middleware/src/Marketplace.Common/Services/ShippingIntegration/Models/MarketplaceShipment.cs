using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Models;
using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
    // these are in the common namespace so that we can reference the FreightPop model
    [SwaggerModel]
    public class MarketplaceShipment : Shipment<ShipmentXp, MarketplaceAddressSupplier, MarketplaceAddressBuyer>
    {
    }

    [SwaggerModel]
    public class ShipmentXp
    {
        // storing full freightPopShipmentRate for potential reference later
        public ShipmentRate FreightPopShipmentRate { get; set; }
        public string Service { get; set; }
    }
}
