using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplacePriceScheduleMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static PriceSchedule Map(MarketplacePriceSchedule m)
        {
            var oc = new PriceSchedule()
            {
                ApplyShipping = m.ApplyShipping,
                ApplyTax = m.ApplyTax,
                ID = m.ID,
                MaxQuantity = m.MaxQuantity,
                MinQuantity = m.MinQuantity,
                Name = m.Name,
                PriceBreaks = m.PriceBreaks,
                RestrictedQuantity = m.RestrictedQuantity,
                UseCumulativeQuantity = m.UseCumulativeQuantity,
                xp = new
                {

                }
            };
            return oc;
        }

        public static MarketplacePriceSchedule Map(PriceSchedule oc)
        {
            var m = new MarketplacePriceSchedule()
            {
                MinQuantity = oc.MinQuantity,
                ApplyShipping = oc.ApplyShipping,
                ApplyTax = oc.ApplyTax,
                ID = oc.ID,
                MaxQuantity = oc.MaxQuantity,
                Name = oc.Name,
                PriceBreaks = oc.PriceBreaks,
                RestrictedQuantity = oc.RestrictedQuantity,
                UseCumulativeQuantity = oc.UseCumulativeQuantity
            };

            return m;
        }

        public static PartialPriceSchedule Map(Partial<MarketplacePriceSchedule> m)
        {
            return m.Values.ToObject<PartialPriceSchedule>();
        }

        public static MarketplaceListPage<MarketplacePriceSchedule> Map(ListPage<PriceSchedule> oc)
        {
            var list = new MarketplaceListPage<MarketplacePriceSchedule>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
