using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
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
		[JsonConverter(typeof(StringEnumConverter))]
		public CurrencySymbol? Currency { get; set; } = null;
    }
}
