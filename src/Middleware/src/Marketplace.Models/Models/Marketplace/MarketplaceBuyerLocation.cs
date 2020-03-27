using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceBuyerLocation
    {
        public MarketplaceUserGroup UserGroup { get; set; }
        public MarketplaceAddressBuyer Address { get; set; }
    }
}
