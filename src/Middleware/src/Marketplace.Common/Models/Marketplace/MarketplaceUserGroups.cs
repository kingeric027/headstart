using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceBuyerLocation
    {
        public MarketplaceLocationUserGroup UserGroup { get; set; }
        public MarketplaceAddressBuyer Address { get; set; }
    }
    [SwaggerModel]
    public class MarketplaceUserGroup : UserGroup<UserGroupXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class UserGroupXp
    {
        public string Type { get; set; }
        public string Role { get; set; }
    }

    [SwaggerModel]
    public class MarketplaceLocationUserGroup : UserGroup<MarketplaceLocationUserGroupXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class MarketplaceLocationUserGroupXp
    {
        public string Type { get; set; }
        public string Role { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencySymbol? Currency { get; set; } = null;
        public string Country { get; set; }
        public List<string> CatalogAssignments { get; set; }
    }
}
