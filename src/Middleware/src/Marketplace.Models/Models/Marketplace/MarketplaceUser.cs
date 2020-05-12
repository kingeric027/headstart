using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceUser : User<UserXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class UserXp
    {
    }
}
