using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS.Models
{
	[SwaggerModel]
	[CosmosCollection("assetcontainers")]
	public class AssetContainer : CosmosObject 
	{
		[JsonProperty("ID"), InteropID]
		public string InteropID { get; set; }
		public string Name { get; set; } // "Assets-{SellerID}"
		public int? MaxiumumSizeBytes { get; set; } = null;
		public string BaseUrl { get; set; }
		[ApiReadOnly]
		public AssetContainerMetadata Metadata { get; set; }
	}

	[SwaggerModel]
	public class AssetContainerMetadata
	{
		public bool IsStorageInternal { get; set; } // stored in cosmos
		public int AssetCount { get; set; } // generated
		public int SizeBytes { get; set; } // generated
	}
}
