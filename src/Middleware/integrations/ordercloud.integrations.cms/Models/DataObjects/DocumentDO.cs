using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using System.Collections.ObjectModel;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("documents")]
	public class DocumentDO : CosmosObject
	{
		public string InteropID { get; set; }
		public string SchemaID { get; set; } // real id, not interop. Don't need to set or return.
		public string SchemaSpecUrl { get; set; }
		[CosmosPartitionKey]
		public string SellerOrgID { get; set; }
		public JObject Doc { get; set; }
		public History History { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID", "/SchemaID", "/SellerOrgID" }}
			};
		}
	}
}
