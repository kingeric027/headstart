using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceUserGroupAssignmentMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static UserGroupAssignment Map(MarketplaceUserGroupAssignment m)
        {
            var oc = new UserGroupAssignment()
            {
                UserGroupID = m.UserGroupID,
                UserID = m.UserID
            };
            return oc;
        }

        public static MarketplaceUserGroupAssignment Map(UserGroupAssignment oc)
        {
            var m = new MarketplaceUserGroupAssignment()
            {
                UserGroupID = oc.UserGroupID,
                UserID = oc.UserID
            };

            return m;
        }

        public static UserGroupAssignment Map(Partial<MarketplaceUserGroupAssignment> m)
        {
            return m.Values.ToObject<UserGroupAssignment>();
        }

        public static MarketplaceListPage<MarketplaceUserGroupAssignment> Map(ListPage<UserGroupAssignment> oc)
        {
            var list = new MarketplaceListPage<MarketplaceUserGroupAssignment>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
