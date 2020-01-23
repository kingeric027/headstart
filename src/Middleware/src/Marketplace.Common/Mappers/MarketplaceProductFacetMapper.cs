using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceProductFacetMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static ProductFacet Map(MarketplaceProductFacet m)
        {
            var oc = new ProductFacet()
            {
                ID = m.ID,
                ListOrder = m.ListOrder,
                MinCount = m.MinCount,
                Name = m.Name,
                XpPath = m.XpPath,
                xp = new
                {
                    m.Options
                }
            };
            return oc;
        }

        public static MarketplaceProductFacet Map(ProductFacet oc)
        {
            var m = new MarketplaceProductFacet()
            {
                ID = oc.ID,
                Name = oc.Name,
                ListOrder = oc.ListOrder,
                MinCount = oc.MinCount,
                XpPath = oc.XpPath,
                Options = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "Options") : null,
            };

            return m;
        }

        public static PartialProductFacet Map(Partial<MarketplaceProductFacet> m)
        {
            return m.Values.ToObject<PartialProductFacet>();
        }

        public static MarketplaceListPage<MarketplaceProductFacet> Map(ListPage<ProductFacet> oc)
        {
            var list = new MarketplaceListPage<MarketplaceProductFacet>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
