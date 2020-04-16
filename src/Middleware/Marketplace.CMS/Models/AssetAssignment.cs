using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS.Models
{
	public enum ResourceType
	{
		Product, Category, Catalog, Promotion, Supplier, Buyer, BuyerAddress, SupplierAddrress,
		AdminAddress, BuyerUserGroup, SupplierUserGroup, AdminUserGroup, Facet
	}

	[SwaggerModel]
	public class AssetAssignment : CosmosObject
	{
		[Required]
		public string ContainderID { get; set; }
		[Required]
		public string AssetID { get; set; }
		[Required]
		public string ResourceID { get; set; }
		[Required]
		public string ResourceType { get; set; }
		public int AssetListOrder { get; set; } // Within the context of a single oc resource 
	}
}
