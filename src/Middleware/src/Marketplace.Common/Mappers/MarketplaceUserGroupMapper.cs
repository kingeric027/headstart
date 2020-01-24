using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceUserGroupMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static UserGroup Map(MarketplaceUserGroup m)
        {
            var oc = new UserGroup()
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

        public static MarketplaceUserGroup Map(UserGroup oc)
        {
            var m = new MarketplaceUserGroup()
            {
                ID = oc.ID,
                Name = oc.Name,
                Description = oc.Description
            };

            return m;
        }

        public static PartialUserGroup Map(Partial<MarketplaceUserGroup> m)
        {
            return m.Values.ToObject<PartialUserGroup>();
        }

        public static MarketplaceListPage<MarketplaceUserGroup> Map(ListPage<UserGroup> oc)
        {
            var list = new MarketplaceListPage<MarketplaceUserGroup>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
