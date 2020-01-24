using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceSpecOptionMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static SpecOption Map(MarketplaceSpecOption m)
        {
            var oc = new SpecOption()
            {
                ID = m.ID,
                ListOrder = m.ListOrder,
                IsOpenText = m.IsOpenText,
                PriceMarkup = m.PriceMarkup,
                PriceMarkupType = m.PriceMarkupType,
                Value = m.Value,
                xp = new
                {
                    m.Description,
                    m.SpecID
                }
            };
            return oc;
        }

        public static MarketplaceSpecOption Map(SpecOption oc)
        {
            var m = new MarketplaceSpecOption()
            {
                ID = oc.ID,
                ListOrder = oc.ListOrder,
                Description = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "Description") : null,
                IsOpenText = oc.IsOpenText,
                PriceMarkup = oc.PriceMarkup,
                PriceMarkupType = oc.PriceMarkupType,
                SpecID = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "SpecID") : null,
                Value = oc.Value,
            };

            return m;
        }

        public static PartialSpecOption Map(Partial<MarketplaceSpecOption> m)
        {
            return m.Values.ToObject<PartialSpecOption>();
        }

        public static MarketplaceListPage<MarketplaceSpecOption> Map(ListPage<SpecOption> oc)
        {
            var list = new MarketplaceListPage<MarketplaceSpecOption>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
