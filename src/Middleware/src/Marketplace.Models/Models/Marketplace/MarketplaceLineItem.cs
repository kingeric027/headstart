using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Models.Models.Marketplace
{
    [SwaggerModel]
	public class MarketplaceLineItem : LineItem<LineItemXp, MarketplaceLineItemProduct, LineItemVariant, MarketplaceAddressBuyer, MarketplaceAddressSupplier> { }

    [SwaggerModel]
	public class LineItemXp 
    {
        public LineItemReturnInfo LineItemReturnInfo { get; set; }
        public decimal? ProductUnitPrice { get; set; }
    }
    public class LineItemReturnInfo
    {
        public int QuantityToReturn { get; set; }
        public string ReturnReason { get; set; }
        public bool Resolved { get; set; }
    }
}
