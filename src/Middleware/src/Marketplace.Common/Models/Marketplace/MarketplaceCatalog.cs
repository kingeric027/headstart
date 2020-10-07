using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceCatalog : UserGroup<CatalogXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class PartialMarketplaceCatalog : PartialUserGroup<CatalogXp>
    {
    }

    // potentially use this for the api later
    [SwaggerModel]
    public class MarketplaceCatalogAssignment : IMarketplaceObject
    {
        // ID not used but to get marketplaceobject extension working for now
        public string ID { get; set; }
        public string LocationID { get; set; }
        public string CatalogID { get; set; }
    }

    [SwaggerModel]
    public class MarketplaceCatalogAssignmentRequest
    {
        public List<string> CatalogIDs { get; set;}
    }

    [SwaggerModel]
    public class CatalogXp
    {
        public string Type { get; set; } = "Catalog";
    }
}
