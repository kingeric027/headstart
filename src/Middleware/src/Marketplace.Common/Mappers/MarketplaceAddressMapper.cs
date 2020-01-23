using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceAddressMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static Address Map(MarketplaceAddress m)
        {
            var oc = new Address()
            {
                ID = m.ID,
                Street1 = m.Street1,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Phone = m.Phone,
                State = m.State,
                Street2 = m.Street2,
                Zip = m.Zip,
                AddressName = m.AddressName,
                City = m.City,
                CompanyName = m.CompanyName,
                Country = m.Country,
                DateCreated = m.DateCreated,
                xp = new
                {
                    
                }
            };
            return oc;
        }

        public static MarketplaceAddress Map(Address oc)
        {
            var m = new MarketplaceAddress()
            {
                ID = oc.ID,
                AddressName = oc.AddressName,
                City = oc.City,
                CompanyName = oc.CompanyName,
                Country = oc.Country,
                DateCreated = oc.DateCreated,
                FirstName = oc.FirstName,
                LastName = oc.LastName,
                Phone = oc.Phone,
                State = oc.State,
                Street1 = oc.Street1,
                Street2 = oc.Street2,
                Zip = oc.Zip
            };

            return m;
        }

        public static PartialAddress Map(Partial<MarketplaceAddress> m)
        {
            return m.Values.ToObject<PartialAddress>();
        }

        public static MarketplaceListPage<MarketplaceAddress> Map(ListPage<Address> oc)
        {
            var list = new MarketplaceListPage<MarketplaceAddress>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
