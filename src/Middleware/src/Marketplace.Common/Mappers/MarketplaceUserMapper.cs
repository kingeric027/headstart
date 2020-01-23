using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents.SystemFunctions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceUserMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static User Map(MarketplaceUser m)
        {
            var oc = new User()
            {
                ID = m.ID,
                Active = m.Active,
                Phone = m.Phone,
                DateCreated = m.DateCreated,
                FirstName = m.FirstName,
                LastName = m.LastName,
                PasswordLastSetDate = m.PasswordLastSetDate,
                TermsAccepted = m.TermsAccepted,
                Username = m.Username,
                AvailableRoles = m.AvailableRoles,
                Email = m.Email,
                Password = m.Password,
                xp = new
                {
                    
                }
            };
            return oc;
        }

        public static MarketplaceUser Map(User oc)
        {
            var m = new MarketplaceUser()
            {
                ID = oc.ID,
                Active = oc.Active,
                AvailableRoles = oc.AvailableRoles,
                DateCreated = oc.DateCreated,
                Email = oc.Email,
                FirstName = oc.FirstName,
                LastName = oc.LastName,
                Phone = oc.Phone,
                Password = oc.Password,
                PasswordLastSetDate = oc.PasswordLastSetDate,
                TermsAccepted = oc.TermsAccepted,
                Username = oc.Username
            };

            return m;
        }

        public static PartialUser Map(Partial<MarketplaceUser> m)
        {
            return m.Values.ToObject<PartialUser>();
        }

        public static MarketplaceListPage<MarketplaceUser> Map(ListPage<User> oc)
        {
            var list = new MarketplaceListPage<MarketplaceUser>
            {
                Items = oc.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(oc.Meta)
            };
            return list;
        }
    }
}
