using Marketplace.Helpers.Attributes;
using Marketplace.Models.Models.Marketplace.Extended;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceUserGroup : UserGroup<UserGroupXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class UserGroupXp
    {
        public string Type { get; set; }
        public Currency Currency { get; set; }
    }
}
