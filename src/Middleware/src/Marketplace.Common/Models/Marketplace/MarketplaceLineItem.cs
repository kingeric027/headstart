using ordercloud.integrations.library;
using OrderCloud.SDK;
using Marketplace.Models.Extended;

namespace Marketplace.Models.Models.Marketplace
{
    [SwaggerModel]
	public class MarketplaceLineItem : LineItem<LineItemXp, MarketplaceLineItemProduct, LineItemVariant, MarketplaceAddressBuyer, MarketplaceAddressSupplier> { }

    [SwaggerModel]
	public class LineItemXp 
    {
        public LineItemStatus LineItemStatus { get; set; }
        public LineItemReturnInfo LineItemReturnInfo { get; set; }
        public string LineItemImageUrl { get; set; }
    }

	[SwaggerModel]
	public class LineItemReturnInfo
    {
        public int QuantityToReturn { get; set; }
        public string ReturnReason { get; set; }
        public string Comment { get; set; }
        public bool Resolved { get; set; }
    }
}
