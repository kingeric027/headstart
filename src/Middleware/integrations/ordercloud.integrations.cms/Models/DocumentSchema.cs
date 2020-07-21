using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	[CosmosCollection("documentschemas")]
	public class DocumentSchema : CosmosObject
	{
		[JsonProperty("ID"), CosmosInteropID, Required]
		public string InteropID { get; set; }
		[CosmosPartitionKey, ApiIgnore]
		public string OwnerClientID { get; set; }
		public List<ResourceType> RestrictedAssignmentTypes { get; set; } = new List<ResourceType>(); // empty means no restrictions
		[Required]
		public JObject Schema { get; set; }
		[ApiReadOnly]
		public History History { get; set; }
		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID", "/OwnerClientID" }}
			};
		}
	}
}
