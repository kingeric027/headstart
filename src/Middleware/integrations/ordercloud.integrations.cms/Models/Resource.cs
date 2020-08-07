using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class Resource
	{
		public Resource() { }
		public Resource(ResourceType type, string id, string parentID = null)
		{
			ResourceType = type;
			ResourceID = id;
			ParentResourceID = parentID;
		}
		[Required]
		public string ResourceID { get; set; }
		public string ParentResourceID { get; set; } = null;
		[Required]
		public ResourceType? ResourceType { get; set; }
	}

	public enum ResourceType
	{
		Products, 
		Categories, 
		Catalogs, 
		Promotions, 
		Suppliers, 
		Buyers, 
		ProductFacets,
		Users, 
		SupplierUsers, 
		AdminUsers, 
		Addresses, 
		SupplierAddresses, 
		AdminAddresses,
		UserGroups,
		SupplierUserGroups,
		AdminUserGroups
	}
}
