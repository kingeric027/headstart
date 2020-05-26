
using ordercloud.integrations.openapispec;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceBuyerLocation
    {
        public MarketplaceUserGroup UserGroup { get; set; }
        public MarketplaceAddressBuyer Address { get; set; }
    }
}
