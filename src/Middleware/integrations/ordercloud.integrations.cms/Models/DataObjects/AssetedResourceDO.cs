using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("assetedresource")]
	public class AssetedResourceDO : CosmosObject
	{
		[CosmosPartitionKey]
		public string SellerOrgID { get; set; }
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; }
		public ResourceType ResourceType { get; set; }
		// Images are separated out so that they can be ordered, including a primary for the thumbnail.
		public List<string> ImageAssetIDs { get; set; } = new List<string>(); 
		public List<string> AllOtherAssetIDs { get; set; } = new List<string>();
		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/SellerOrgID", "/ResourceID", "/ResourceType", "/ResourceParentID" }}
			};
		}
	}
}
