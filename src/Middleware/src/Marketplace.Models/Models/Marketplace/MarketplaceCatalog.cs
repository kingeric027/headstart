
using ordercloud.integrations.openapispec;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceCatalog : Catalog<CatalogXp>, IMarketplaceObject
    {
        
    }

    [SwaggerModel]
    public class CatalogXp
    {
    }
}
