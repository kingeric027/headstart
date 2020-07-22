using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("assetedresource")]
	public class AssetedResource : CosmosObject
	{
		[CosmosPartitionKey]
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; }
		public ResourceType ResourceType { get; set; }
		public List<string> ImageAssetIDs { get; set; } = new List<string>();
		public List<string> ThemeAssetIDs { get; set; } = new List<string>();
		public List<string> AttachmentAssetIDs { get; set; } = new List<string>();
		public List<string> StructuredAssetsIDs { get; set; } = new List<string>();

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/ResourceID", "/ResourceType", "/ResourceParentID" }}
			};
		}
	}
}
