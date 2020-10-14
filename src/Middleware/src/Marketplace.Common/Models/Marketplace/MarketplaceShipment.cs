using Marketplace.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
    [SwaggerModel]
    public class SuperShipment
    {
        public MarketplaceShipment Shipment { get; set; }
        public List<ShipmentItem> ShipmentItems { get; set; }
    }

    // these are in the common namespace so that we can reference the FreightPop model
    [SwaggerModel]
    public class MarketplaceShipment : Shipment<ShipmentXp, MarketplaceAddressSupplier, MarketplaceAddressBuyer>
    {
    }

    [SwaggerModel]
    public class ShipmentXp
    {
        // storing full freightPopShipmentRate for potential reference later
        //public ShipmentRate FreightPopShipmentRate { get; set; }
        public string Service { get; set; }
        public string Comment { get; set; }
    }

    [SwaggerModel]
    public class MarketplaceShipmentWithItems : Shipment
    {
        public List<MarketplaceShipmentItemWithLineItem> ShipmentItems { get; set; }
    }

    [SwaggerModel]
    public class MarketplaceShipmentItemWithLineItem : ShipmentItem
    {
        public LineItem LineItem { get; set; }
    }
}
