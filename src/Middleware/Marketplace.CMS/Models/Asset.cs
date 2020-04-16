using System;
using System.Collections.Generic;
using System.Text;
using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Newtonsoft.Json;
using OrderCloud.SDK;

namespace Marketplace.CMS.Models
{
	public enum AssetType { Image, Theme, Attachment, StructuredData }

	[SwaggerModel]
	[CosmosCollection("assets")]
	public class Asset : CosmosObject
	{
		[JsonProperty("ID"), InteropID]
		public string InteropID { get; set; }
		[Required]
		public string ContainerID { get; set; } // Don't need to set or return. Only goes into building the Url.
		public string Url { get; set; } // Settable to support external storage. Generated if not set. 
		public string Title { get; set; }
		public List<string> Tags { get; set; }
		public AssetType Type { get; set; }
		public string FileName { get; set; } // Defaults to the file name in the upload. Or should be required?
		[ApiReadOnly]
		public AssetMetadata Metadata { get; set; }
	}

	[SwaggerModel]
	public class AssetMetadata
	{
		public string ContentType { get; set; }
		public int SizeBytes { get; set; }
		public bool IsUrlOverridden { get; set; } // true if Url was set, false if it was generated
		public int? ImageHeight { get; set; } = null; // null if asset not image
		public int? ImageWidth { get; set; } = null; // null if asset not image
	}
}
