using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Models.Models.Misc
{
	public enum AssetType { Image, Theme, Attachment, StructuredData }

	public enum ResourceType
	{
		Product, Category, Catalog, Promotion, Supplier, Buyer, BuyerAddress, SupplierAddrress,
		AdminAddress, BuyerUserGroup, SupplierUserGroup, AdminUserGroup, Facet
	}

	[SwaggerModel]
	public class Asset: CosmosObject
	{
		[Required]
		public string AssetContainerID { get; set; } // Don't need to set or return. Only goes into building the Url.
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

	[SwaggerModel]
	public class AssetAssignment: CosmosObject
	{
		[Required]
		public string ContainderID { get; set; }
		[Required]
		public string AssetID { get; set; }
		[Required]
		public string ResourceID { get; set; }
		[Required]
		public string ResourceType { get; set; }
		public int AssetListOrder { get; set; } // Within the context of a single oc resource 
	}

	[SwaggerModel]
	public class AssetContainer: CosmosObject
	{
		public string Name { get; set; } // "Assets-{SellerID}"
		public int MaxiumumSizeBytes { get; set; }
		public string BaseUrl { get; set; }
		[ApiReadOnly]
		public ContainerMetadata Metadata { get; set; }
	}

	[SwaggerModel]
	public class ContainerMetadata
	{
		public bool IsStorageInternal { get; set; } // stored in cosmos
		public int AssetCount { get; set; } // generated
		public int SizeBytes { get; set; } // generated
	}
}
