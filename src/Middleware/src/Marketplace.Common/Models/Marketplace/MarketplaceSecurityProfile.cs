using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
	public class MarketplaceSecurityProfile
	{
		public CustomRole CustomRole { get; set; }
		public ApiRole[] Roles { get; set; }
	}
}
