﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
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
		[Required, ApiIgnore, CosmosPartitionKey]
		public string ContainerID { get; set; } // real id, not interop. Don't need to set or return.
		public string Title { get; set; }
		public bool Active { get; set; } = false;
		[ApiReadOnly, CosmosIgnore]
		public string Url { get; set; } // Generated if not set. 
		public string UrlPathOveride { get; set; } = null; // saved
		public AssetType? Type { get; set; } = null;
		public List<string> Tags { get; set; }
		public string FileName { get; set; } // Defaults to the file name in the upload. Or should be required?
		[ApiReadOnly]
		public AssetMetadata Metadata { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID" }}
			};
		}
	}

	[SwaggerModel]
	public class AssetMetadata
	{
		public string ContentType { get; set; }
		public long? SizeBytes { get; set; }
		public int? ImageHeight { get; set; } = null; // null if asset not image
		public int? ImageWidth { get; set; } = null; // null if asset not image
		public float? ImageVerticalResolution { get; set; } = null; // pixels per inch
		public float? ImageHorizontalResolution { get; set; } = null; // pixels per inch
	}
}