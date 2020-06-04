using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceCatalogAssignment : CatalogAssignment, IMarketplaceObject
    {
        public string ID { get; set; }
    }
}
