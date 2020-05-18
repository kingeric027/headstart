using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceSpecOption : SpecOption<SpecOptionXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class SpecOptionXp
    {
        public string Description { get; set; }
        public string SpecID { get; set; }
    }
}
