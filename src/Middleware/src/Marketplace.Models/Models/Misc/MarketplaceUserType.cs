using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.openapispec;

namespace Marketplace.Models.Misc
{
    [SwaggerModel]
	public class MarketplaceUserType
	{
		public string UserGroupIDSuffix { get; set; }
		public string UserGroupName { get; set; }
		public UserGroupType UserGroupType { get; set; }
		public List<CustomRole> CustomRoles { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum UserGroupType
	{
		UserPermissions,
		BuyerLocation,
		LocationPermissions
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum UserGroupSuffix
	{
		OrderApprover,
		PermissionAdmin,
		NeedsApproval,
		ViewAllOrders,
		CreditCardAdmin,
		AddressAdmin

	}
}