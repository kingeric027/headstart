using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceAddressAssignmentMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static AddressAssignment Map(MarketplaceAddressAssignment m)
        {
            var oc = new AddressAssignment()
            {
                AddressID = m.AddressID,
                UserGroupID = m.UserGroupID,
                IsBilling = m.IsBilling,
                UserID = m.UserID,
                IsShipping = m.IsShipping
            };
            return oc;
        }

        public static MarketplaceAddressAssignment Map(AddressAssignment oc)
        {
            var m = new MarketplaceAddressAssignment()
            {
                AddressID = oc.AddressID,
                UserGroupID = oc.UserGroupID,
                IsBilling = oc.IsBilling,
                IsShipping = oc.IsShipping,
                UserID = oc.UserID
            };

            return m;
        }

        public static AddressAssignment Map(Partial<MarketplaceAddressAssignment> m)
        {
            return m.Values.ToObject<AddressAssignment>();
        }

        public static MarketplaceListPage<MarketplaceAddressAssignment> Map(ListPage<AddressAssignment> oc)
        {
            var list = new MarketplaceListPage<MarketplaceAddressAssignment>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
