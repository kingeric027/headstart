using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ordercloud.integrations.cms
{
	public enum AssetType { Image, Text, Audio, Video, Slides, SpreadSheet, PDF, Compressed, Code, JSON, Markup, Unknown }

	[SwaggerModel]
	public class Asset
	{
		[CosmosInteropID]
		public string ID { get; set; }
		[MaxLength(100)]
		public string Title { get; set; }
		public bool Active { get; set; } = false;
		public string Url { get; set; } // Generated if not set. 
		public AssetType Type { get; set; }
		public List<string> Tags { get; set; }
		public string FileName { get; set; } // Defaults to the file name in the upload. Or should be required?
		[ApiReadOnly]
		public AssetMetadata Metadata { get; set; }
		[ApiReadOnly]
		public History History { get; set; }
	}

	[SwaggerModel]
	public class AssetMetadata
	{
		public bool IsUrlOverridden { get; set; } = false;
		public string ContentType { get; set; }
		public int? SizeBytes { get; set; }
		public int? ImageHeight { get; set; } = null; // null if asset not image
		public int? ImageWidth { get; set; } = null; // null if asset not image
		public decimal? ImageVerticalResolution { get; set; } = null; // pixels per inch
		public decimal? ImageHorizontalResolution { get; set; } = null; // pixels per inch
	}
}
