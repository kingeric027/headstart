using Marketplace.Helpers.Attributes;
using Marketplace.Models.Extended;
using OrderCloud.SDK;
using System.Collections.Generic;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

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
