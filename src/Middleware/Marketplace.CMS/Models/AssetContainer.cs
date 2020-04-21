using Cosmonaut.Attributes;
using Marketplace.CMS.Queries;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Marketplace.CMS.Models
{
	[SwaggerModel]
	[CosmosCollection("assetcontainers")]
	public class AssetContainer : CosmosObject 
	{
		[CosmosPartitionKey, ApiIgnore]
		public string SinglePartitionID => AssetContainerQuery.SinglePartitionID; // TODO - is there a better way to indicate there should only be one partition?
		[JsonProperty("ID"), InteropID]
		public string InteropID { get; set; }
		[Required]
		public string Name { get; set; }
		public string HostUrlOverride { get; set; } = null;
		[ApiReadOnly]
		public bool StorageConnected { get; set; } = false;
		[ApiReadOnly]
		public StorageAccount StorageAccount { get; set; } = null;
		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID" }}
			};
		}
	}

	public enum StorageAccountType { DefaultBlob }; // ExternalBlob, ExternalS3

	[SwaggerModel]
	public class StorageAccount
	{
		public StorageAccountType Type { get; set; } 
		public string HostUrl { get; set; }
		public string ConnectionString { get; set; } // Need to figure out good security for this. Same as user password?
	}
}
