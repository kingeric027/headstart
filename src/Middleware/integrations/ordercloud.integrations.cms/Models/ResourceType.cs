using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public enum ResourceType
	{
		Catalogs,
		[ParentResource(Catalogs)] Categories,
		Products,
		PriceSchedules,
		ProductFacets,
		Specs,

		SecurityProfiles,
		PasswordResets,
		OpenIdConnects,
		ImpersonationConfigs,

		Buyers,
		[ParentResource(Buyers)] Users,
		[ParentResource(Buyers)] UserGroups,
		[ParentResource(Buyers)] Addresses,
		[ParentResource(Buyers)] CostCenters,
		[ParentResource(Buyers)] CreditCards,
		[ParentResource(Buyers)] SpendingAccounts,
		[ParentResource(Buyers)] ApprovalRules,

		Suppliers,
		[ParentResource(Suppliers)] SupplierUsers,
		[ParentResource(Suppliers)] SupplierUserGroups,
		[ParentResource(Suppliers)] SupplierAddresses,

		// Param "Direction" breaks these for now.
		//Orders,
		// [ParentResource(Orders)] LineItems,
		// [ParentResource(Orders)] Payments,
		// [ParentResource(Orders)]Shipments,
		Promotions,

		AdminUsers,
		AdminAddresses,
		AdminUserGroups,
		MessageSenders,
		Webhooks,
		ApiClients,
		Incrementors,
		IntegrationEvents,
		XpIndices
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ParentResourceAttribute: Attribute
	{
		public ResourceType ParentType { get; set; }
		public ParentResourceAttribute(ResourceType type)
		{
			ParentType = type;
		}
	}
}
