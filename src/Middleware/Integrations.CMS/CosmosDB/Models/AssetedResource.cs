using Cosmonaut.Attributes;
using Integrations.CMS.Models;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Marketplace.CMS.Models
{
	[CosmosCollection("assetedresource")]
	public class AssetedResource : CosmosObject
	{
		public Resource Resource { get; set; }
		public List<string> ImageAssetIDs { get; set; } = new List<string>();
		public List<string> OtherAssetIDs { get; set; } = new List<string>();

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/Resource/ID", "/Resource/Type", "/Resource/ParentID" }}
			};
		}
	}

	public class Resource
	{
		public Resource(ResourceType type, string id = null, string parentID = null)
		{
			Type = type;
			ID = id;
			ParentID = parentID;
		}

		[CosmosPartitionKey]
		public string ID { get; set; }
		public string ParentID { get; set; } = null;
		public ResourceType Type { get; set; }
	}

	public enum ResourceType
	{
		Products, Categories, Catalogs, Promotions, Suppliers, Buyers, ProductFacets
	}

}
