using Cosmonaut.Attributes;
using Marketplace.CMS.Queries;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using OrderCloud.SDK;
using System;
using System.Collections.ObjectModel;
using ordercloud.integrations.cosmos;
using ordercloud.integrations.openapispec;

namespace Marketplace.CMS.Models
{
	[SwaggerModel]
	[CosmosCollection("assetcontainers")]
	public class AssetContainer : CosmosObject 
	{
		[CosmosPartitionKey, ordercloud.integrations.extensions.ApiIgnore]
		public string SinglePartitionID => AssetContainerQuery.SinglePartitionID; // TODO - is there a better way to indicate there should only be one partition?
		[JsonProperty("ID"), InteropID]
		public string InteropID { get; set; }
		[Required]
		public string Name { get; set; }
		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID" }}
			};
		}
	}
}
