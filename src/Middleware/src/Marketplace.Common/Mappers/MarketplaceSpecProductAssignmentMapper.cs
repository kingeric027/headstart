using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceSpecProductAssignmentMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static SpecProductAssignment Map(MarketplaceSpecProductAssignment m)
        {
            var oc = new SpecProductAssignment()
            {
                ProductID = m.ProductID,
                SpecID = m.SpecID,
                DefaultOptionID = m.DefaultOptionID,
                DefaultValue = m.DefaultValue
            };
            return oc;
        }

        public static MarketplaceSpecProductAssignment Map(SpecProductAssignment oc)
        {
            var m = new MarketplaceSpecProductAssignment()
            {
                ProductID = oc.ProductID,
                SpecID = oc.SpecID,
                DefaultOptionID = oc.DefaultOptionID,
                DefaultValue = oc.DefaultValue
            };

            return m;
        }

        public static SpecProductAssignment Map(Partial<MarketplaceSpecProductAssignment> m)
        {
            return m.Values.ToObject<SpecProductAssignment>();
        }

        public static MarketplaceListPage<MarketplaceSpecProductAssignment> Map(ListPage<SpecProductAssignment> oc)
        {
            var list = new MarketplaceListPage<MarketplaceSpecProductAssignment>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
