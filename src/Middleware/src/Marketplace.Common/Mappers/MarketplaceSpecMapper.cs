using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceSpecMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static Spec Map(MarketplaceSpec m)
        {
            var oc = new Spec()
            {
                ID = m.ID,
                ListOrder = m.ListOrder,
                Name = m.Name,
                AllowOpenText = m.AllowOpenText,
                DefaultOptionID = m.DefaultOptionID,
                DefaultValue = m.DefaultValue,
                DefinesVariant = m.DefinesVariant,
                OptionCount = m.OptionCount,
                Required = m.Required,
                xp = new
                {
                    m.UI
                }
            };
            return oc;
        }

        public static MarketplaceSpec Map(Spec oc)
        {
            var m = new MarketplaceSpec()
            {
                ID = oc.ID,
                ListOrder = oc.ListOrder,
                Name = oc.Name,
                DefaultOptionID = oc.DefaultOptionID,
                DefaultValue = oc.DefaultValue,
                AllowOpenText = oc.AllowOpenText,
                DefinesVariant = oc.DefinesVariant,
                OptionCount = oc.OptionCount,
                Options = oc.Options,
                Required = oc.Required,
                UI = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "UI") : null
            };

            return m;
        }

        public static PartialSpec Map(Partial<MarketplaceSpec> m)
        {
            return m.Values.ToObject<PartialSpec>();
        }

        public static MarketplaceListPage<MarketplaceSpec> Map(ListPage<Spec> oc)
        {
            var list = new MarketplaceListPage<MarketplaceSpec>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
