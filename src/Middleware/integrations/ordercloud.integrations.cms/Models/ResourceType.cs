using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public enum ResourceType
	{
		Categories,
		[ParentType(Categories)] Catalogs,
		Products,
		PriceSchedules,
		ProductFacets,
		Specs,

		SecurityProfiles,
		PasswordResets,
		OpenIdConnects,
		ImpersonationConfigs,

		Buyers,
		[ParentType(Buyers)] Users,
		[ParentType(Buyers)] UserGroups,
		[ParentType(Buyers)] Addresses,
		[ParentType(Buyers)] CostCenters,
		[ParentType(Buyers)] CreditCards,
		[ParentType(Buyers)] SpendingAccounts,
		[ParentType(Buyers)] ApprovalRules,

		Suppliers,
		[ParentType(Suppliers)] SupplierUsers,
		[ParentType(Suppliers)] SupplierUserGroups,
		[ParentType(Suppliers)] SupplierAddresses,

		// Param "Direction" breaks these for now.
		//Orders,
		//LineItems,
		//Payments,
		//Shipments,
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
	public class ParentTypeAttribute: Attribute
	{
		public ResourceType ParentType { get; set; }
		public ParentTypeAttribute(ResourceType type)
		{
			ParentType = type;
		}
	}
}
