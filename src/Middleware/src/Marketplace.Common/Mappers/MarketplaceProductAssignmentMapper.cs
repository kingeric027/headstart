using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceProductAssignmentMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static ProductAssignment Map(MarketplaceProductAssignment m)
        {
            var oc = new ProductAssignment()
            {
                BuyerID = m.BuyerID,
                PriceScheduleID = m.PriceScheduleID,
                ProductID = m.ProductID,
                UserGroupID = m.UserGroupID,
            };
            return oc;
        }

        public static MarketplaceProductAssignment Map(ProductAssignment oc)
        {
            var m = new MarketplaceProductAssignment()
            {
                BuyerID = oc.BuyerID,
                PriceScheduleID = oc.PriceScheduleID,
                ProductID = oc.ProductID,
                UserGroupID = oc.UserGroupID
            };

            return m;
        }

        public static ProductAssignment Map(Partial<MarketplaceProductAssignment> m)
        {
            return m.Values.ToObject<ProductAssignment>();
        }

        public static MarketplaceListPage<MarketplaceProductAssignment> Map(ListPage<ProductAssignment> oc)
        {
            var list = new MarketplaceListPage<MarketplaceProductAssignment>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
