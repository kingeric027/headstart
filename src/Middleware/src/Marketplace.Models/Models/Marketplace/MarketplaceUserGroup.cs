﻿using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceUserGroup : UserGroup<UserGroupXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class UserGroupXp
    {
        public string Type { get; set; }
        public string Role { get; set; }
        public string Currency { get; set; }
    }
}
