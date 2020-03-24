using System.Collections.Generic;

namespace Marketplace.Models.Misc
{
	public class MarketplaceUserType
	{
		public string UserGroupIDSuffix { get; set; }
		public string UserGroupName { get; set; }
		public List<CustomRole> CustomRoles { get; set; }
	}
}