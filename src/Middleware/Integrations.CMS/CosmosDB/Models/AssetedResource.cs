using Cosmonaut.Attributes;
using Integrations.CMS.Models;
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
		AdminAddresses, UserGroup, SupplierUserGroups, AdminUserGroups, ProductFacets
	}

	[CosmosCollection("assetedresource")]
	public class AssetedResource : CosmosObject
	{
		public ResourceType ResourceType { get; set; }
		[CosmosPartitionKey]
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; }
		public List<string> ImageAssetIDs { get; set; } = new List<string>();
		public List<string> OtherAssetIDs { get; set; } = new List<string>();

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/ResourceID", "/ResourceType", "/ResourceParentID" }}
			};
		}
	}
}
