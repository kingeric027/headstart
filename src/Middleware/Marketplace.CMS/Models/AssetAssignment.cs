using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Marketplace.CMS.Models
{
	public enum ResourceType
	{
		Products, Categories, Catalogs, Promotions, Suppliers, Buyers, Addresses, SupplierAddrresses,
		AdminAddresses, UserGrousp, SupplierUserGroups, AdminUserGroups, ProductFacets
	}

	[SwaggerModel]
	[CosmosCollection("assetassignments")]
	public class AssetAssignment : CosmosObject
	{
		[ApiIgnore, CosmosPartitionKey]
		public string ContainerID { get; set; }
		[Required]
		public string AssetID { get; set; }
		[Required]
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; }
		[Required]
		public ResourceType ResourceType { get; set; }
		public int AssetListOrder { get; set; } // Within the context of a single oc resource 

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/AssetID", "/ResourceID", "/ResourceType", "/ResourceParentID" }}
			};
		}
	}
}
