using ordercloud.integrations.library;
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
