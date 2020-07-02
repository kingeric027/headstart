using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ordercloud.integrations.cms.Models
{
	[SwaggerModel]
	[CosmosCollection("documentschemas")]
	public class DocumentSchema : CosmosObject
	{
		[JsonProperty("ID"), CosmosInteropID, Required]
		public string InteropID { get; set; }
		[MaxLength(100)]
		public string Name { get; set; }
		[CosmosPartitionKey, Required]
		public string SellerOrgID { get; set; } // This field only needs to be public until it can be read from a token or /me 

		[Required]
		public List<ResourceType> AllowedResourceAssociations { get; set; } // Cannot be empty
		[Required]
		public JObject Schema { get; set; }
		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID" }}
			};
		}
	}
}
