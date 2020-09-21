using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;

namespace Marketplace.Common.Models.Marketplace
{
    public class MarketplaceLineItemOrder
    {
        public MarketplaceOrder MarketplaceOrder { get; set; }
        public MarketplaceLineItem MarketplaceLineItem { get; set; }
    }
}
