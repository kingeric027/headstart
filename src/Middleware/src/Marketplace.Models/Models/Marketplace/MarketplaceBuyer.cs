using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceBuyer : Buyer<BuyerXp>, IMarketplaceObject
    {
        
    }

    [SwaggerModel]
    public class BuyerXp
    {
    }
}
