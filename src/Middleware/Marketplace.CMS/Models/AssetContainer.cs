using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS.Models
{
	[SwaggerModel]
	public class AssetContainer : CosmosObject
	{
		[JsonProperty(PropertyName = "ID")]
		public string InteropID { get; set; }
		public string Name { get; set; } // "Assets-{SellerID}"
		public int MaxiumumSizeBytes { get; set; }
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
