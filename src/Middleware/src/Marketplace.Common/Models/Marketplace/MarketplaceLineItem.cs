using ordercloud.integrations.library;
using OrderCloud.SDK;
using Marketplace.Models.Extended;
using System.Collections.Generic;

namespace Marketplace.Models.Models.Marketplace
{
    [SwaggerModel]
	public class MarketplaceLineItem : LineItem<LineItemXp, MarketplaceLineItemProduct, LineItemVariant, MarketplaceAddressBuyer, MarketplaceAddressSupplier> { }

    [SwaggerModel]
	public class LineItemXp 
    {
        public Dictionary<LineItemStatus, int> StatusByQuantity { get; set; }
        public List<LineItemClaim> Returns { get; set; }
        public List<LineItemClaim> Cancelations { get; set; }
        public string ImageUrl { get; set; }
        public string PrintArtworkURL { get; set; }

        // kit specific fields
        public string KitProductImageUrl { get; set; }
        public string KitProductID { get; set; }
        public string KitProductName { get; set; }
    }

    [SwaggerModel]
    public class LineItemClaim
    {
        public string RMANumber { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
        public bool IsResolved { get; set; }
    }
}
