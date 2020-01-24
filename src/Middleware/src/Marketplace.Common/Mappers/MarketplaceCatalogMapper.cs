using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceCatalogMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static Catalog Map(MarketplaceCatalog m)
        {
            var oc = new Catalog()
            {
                ID = m.ID,
                Name = m.Name,
                Active = m.Active,
                Description = m.Description,
                xp = new
                {
                    
                }
            };
            return oc;
        }

        public static MarketplaceCatalog Map(Catalog oc)
        {
            var m = new MarketplaceCatalog()
            {
                ID = oc.ID,
                Name = oc.Name,
                Active = oc.Active,
                CategoryCount = oc.CategoryCount,
                Description = oc.Description,
                OwnerID = oc.OwnerID
            };

            return m;
        }

        public static PartialCatalog Map(Partial<MarketplaceCatalog> m)
        {
            return m.Values.ToObject<PartialCatalog>();
        }

        public static MarketplaceListPage<MarketplaceCatalog> Map(ListPage<Catalog> oc)
        {
            var list = new MarketplaceListPage<MarketplaceCatalog>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
