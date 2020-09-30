using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.library;

namespace Marketplace.Common.Models.Marketplace
{
    [SwaggerModel]
    public class MarketplaceLineItemOrder
    {
        public MarketplaceOrder MarketplaceOrder { get; set; }
        public MarketplaceLineItem MarketplaceLineItem { get; set; }
    }
}
