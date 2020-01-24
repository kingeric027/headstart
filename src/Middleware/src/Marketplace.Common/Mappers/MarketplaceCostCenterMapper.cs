using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceCostCenterMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static CostCenter Map(MarketplaceCostCenter m)
        {
            var oc = new CostCenter()
            {
                ID = m.ID,
                Name = m.Name,
                Description = m.Description,
                xp = new
                {
                    
                }
            };
            return oc;
        }

        public static MarketplaceCostCenter Map(CostCenter oc)
        {
            var m = new MarketplaceCostCenter()
            {
                ID = oc.ID,
                Name = oc.Name,
                Description = oc.Description
            };

            return m;
        }

        public static PartialCostCenter Map(Partial<MarketplaceCostCenter> m)
        {
            return m.Values.ToObject<PartialCostCenter>();
        }

        public static MarketplaceListPage<MarketplaceCostCenter> Map(ListPage<CostCenter> oc)
        {
            var list = new MarketplaceListPage<MarketplaceCostCenter>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
