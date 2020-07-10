using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("assetcontainers")]
	public class AssetContainer : CosmosObject 
	{
		[CosmosPartitionKey, ApiIgnore]
		public string SinglePartitionID => AssetContainerQuery.SinglePartitionID; // TODO - is there a better way to indicate there should only be one partition?
		[JsonProperty("ID"), CosmosInteropID]
		public string InteropID { get; set; }
		[Required, MaxLength(100)]
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
