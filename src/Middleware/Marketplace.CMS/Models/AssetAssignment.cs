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
		Product, Category, Catalog, Promotion, Supplier, Buyer, BuyerAddress, SupplierAddrress,
		AdminAddress, BuyerUserGroup, SupplierUserGroup, AdminUserGroup, Facet
	}

	[SwaggerModel]
	[CosmosCollection("assetassignments")]
	public class AssetAssignment : CosmosObject
	{
		[Required, CosmosPartitionKey]
		public string ContainerID { get; set; }
		[Required]
		public string AssetID { get; set; }
		[Required]
		public string ResourceID { get; set; }
		[Required]
		public string ResourceType { get; set; }
		public int AssetListOrder { get; set; } // Within the context of a single oc resource 

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/AssetID", "/ResourceID", "/ResourceType" }}
			};
		}
	}
}
