using OrderCloud.SDK;
using System.Collections.Generic;
using ordercloud.integrations.library;

namespace Marketplace.Models
{
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
