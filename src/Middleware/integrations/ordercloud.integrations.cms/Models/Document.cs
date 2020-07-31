using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using System.Collections.ObjectModel;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	[CosmosCollection("documents")]
	public class Document<TDoc>: CosmosObject
	{
		[JsonProperty("ID"), CosmosInteropID, Required]
		public string InteropID { get; set; }
		[ApiIgnore]
		public string SchemaID { get; set; } // real id, not interop. Don't need to set or return.
		[JsonProperty("$schema"), ApiReadOnly]
		public string SchemaSpecUrl { get; set; }
		[CosmosPartitionKey, ApiIgnore]
		public string OwnerClientID { get; set; }
		[Required]
		public TDoc Doc { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID", "/SchemaID", "/OwnerClientID" }}
			};
		}
	}
}
