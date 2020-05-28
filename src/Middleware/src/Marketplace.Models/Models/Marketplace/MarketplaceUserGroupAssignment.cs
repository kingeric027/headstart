using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceUserGroupAssignment : UserGroupAssignment, IMarketplaceObject
    {
        public string ID { get; set; }
    }
}
