using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceBuyerMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static Buyer Map(MarketplaceBuyer m)
        {
            var oc = new Buyer()
            {
                ID = m.ID,
                Name = m.Name,
                Active = m.Active,
                DefaultCatalogID = m.DefaultCatalogID,
                xp = new
                {
                    
                }
            };
            return oc;
        }

        public static MarketplaceBuyer Map(Buyer oc)
        {
            var m = new MarketplaceBuyer()
            {
                ID = oc.ID,
                Name = oc.Name,
                Active = oc.Active,
                DefaultCatalogID = oc.DefaultCatalogID
            };

            return m;
        }

        public static PartialBuyer Map(Partial<MarketplaceBuyer> m)
        {
            return m.Values.ToObject<PartialBuyer>();
        }

        public static MarketplaceListPage<MarketplaceBuyer> Map(ListPage<Buyer> oc)
        {
            var list = new MarketplaceListPage<MarketplaceBuyer>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
