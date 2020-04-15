using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Models.Models.Misc
{
	public enum AssetType { Image, Theme, Attachment, StructuredData }

	public class Asset: CosmosObject
	{
		[Required]
		public string AssetStorageID { get; set; } // Don't need to set or return. Only goes into building the Url.
		public string Url { get; set; } // Settable to support external storage. Generated if not set. 
		public string Title { get; set; }
		public List<string> Tags { get; set; }
		public AssetType Type { get; set; }
		public string FileName { get; set; } // Defaults to the file name in the upload. Or should be required?
		[ApiReadOnly]
		public AssetMetadata Metadata { get; set; }
	}

	public class AssetAssignment: CosmosObject
	{
		[Required]
		public string AssetID { get; set; }
		[Required]
		public string ResourceID { get; set; }
		[Required]
		public string ResourceType { get; set; }
		public int AssetListOrder { get; set; } // Wthin the content of a single oc resource 
	}

	public class AssetMetadata
	{
		public string ContentType { get; set; }
		public int SizeBytes { get; set; }
		public bool IsStoredExternally { get; set; } // true if Url was set, false if it was generated
		public int? ImageHeight { get; set; } = null; // null if asset not image
		public int? ImageWidth { get; set; } = null; // null if asset not image
	}

	public class AssetStorage: CosmosObject
	{
		public int AssetCount { get; set; } // generated
		public int SizeBytes { get; set; } // generated
		public int MaxiumumSizeBytes { get; set; }
		public string ContainerName { get; set; } // "Assets-{SellerID}"
		public string BlobStorageBaseUrl { get; set; }
	}

	public enum ResourceType
	{
		Product, Category, Catalog, Promotion, Supplier, Buyer, BuyerAddress, SupplierAddrress,
		AdminAddress, BuyerUserGroup, SupplierUserGroup, AdminUserGroup, Facet
	}

	//public class AssetGroup : CosmosObject
	//{
	//	public string Name { get; set; }
	//	public ResourceType ResourceType { get; set; }
	//	public string ResourceID { get; set; }           // Each asset group is part of exactly 1 OC Resource. Is this ok? many-to-many assignment instead?
	//	public int SizeBytes { get; set; } = 0;
	//	public int? MaxSizeBytes { get; set; } = null;
	//	public int? MaxAssetCount { get; set; } = null;
	//	public AssetType? RestrictAssetType { get; set; } = null;
	//	public List<string> AssetIDs { get; set; }       // have a route where these are expanded to include the full object, graphQL-like
	//}




	//public class JSONContent: CosmosObject
	//{
	//	public string ContentTypeID { get; set; }
	//	public dynamic JSON { get; set; } // is dynamic the right type?
	//}

	//public class FileContentAssignment : CosmosObject
	//{
	//	public string FileContentID { get; set; }

	//	public string ContentTypeID { get; set; }
	//}

	//public class JSONContentAssignment : CosmosObject
	//{
	//	public string JSONContentID { get; set; }
	//	public OCResourceType OCResourceType { get; set; }
	//	public string OCResourceID { get; set; }
	//}

	//public class ContentType : CosmosObject
	//{
	//	public string Name { get; set; } // matches ID
	//	public ????? JSONSchema    // How to do this? Is it worth it?
	//}

	// OC Resources that content can be assigned to.

}
