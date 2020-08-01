using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("assets")]
	public class AssetDO : CosmosObject
	{
		public string InteropID { get; set; }
		[CosmosPartitionKey]
		public string ContainerID { get; set; } // real id, not interop. Don't need to set or return.
		public string Title { get; set; }
		public bool Active { get; set; } = false;
		public string Url { get; set; } // Generated if not set. 
		public AssetType Type { get; set; }
		public List<string> Tags { get; set; }
		public string FileName { get; set; } // Defaults to the file name in the upload. Or should be required?
		public AssetMetadata Metadata { get; set; }
		public History History { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID", "/ContainerID" }}
			};
		}
	}
}
