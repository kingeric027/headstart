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
		public string Name { get; set; } // "Assets-{SellerID}"
		public string BaseUrl { get; set; }
		public int? MaxiumumSizeBytes { get; set; } = null;
		[ApiReadOnly]
		public AssetContainerMetadata Metadata { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID" }}
			};
		}
	}

	[SwaggerModel]
	public class AssetContainerMetadata
	{
		public bool IsStorageInternal { get; set; } // stored in cosmos. How to do this?
		public int AssetCount { get; set; } // generated
		public int SizeBytes { get; set; } // generated
	}

}
