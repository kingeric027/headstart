using System.Collections.Generic;
using Marketplace.Models.Models.Misc;
namespace Marketplace.Common.Models
{
	public class MarketplaceUserType
	{
		public string UserGroupIDSuffix { get; set; }
		public string UserGroupName { get; set; }
		public List<CustomRole> CustomRoles { get; set; }
	}
}