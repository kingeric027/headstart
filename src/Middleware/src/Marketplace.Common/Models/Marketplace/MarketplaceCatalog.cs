using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceCatalog : UserGroup<CatalogXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class CatalogXp
    {
        public string Type { get; set; } = "Catalog";
    }
}
