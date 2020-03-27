using System.Collections.Generic;
using Marketplace.Helpers.Attributes;

namespace Marketplace.Models.Misc
{
    [SwaggerModel]
	public class MarketplaceUserType
	{
		public string UserGroupIDSuffix { get; set; }
		public string UserGroupName { get; set; }
		public List<CustomRole> CustomRoles { get; set; }
	}
}