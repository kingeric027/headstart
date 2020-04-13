using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Models.Models.Misc
{
	public class FileContent: CosmosObject
	{
		public string Url { get; set; }
		public string Title { get; set; }
		public List<string> Tags { get; set; }
		public bool IsImage { get; set; } = false;
		[ApiReadOnly]
		public string FileName { get; set; }
		[ApiReadOnly]
		public string FileType { get; set; }
		[ApiReadOnly]
		public decimal FileTypeMB { get; set; }
	}

	public class JSONContent: CosmosObject
	{
		public string ContentTypeID { get; set; }
		public dynamic JSON { get; set; } // is dynamic the right type?
	}

	public class FileContentAssignment : CosmosObject
	{
		public string FileContentID { get; set; }
		public OCResourceType OCResourceType { get; set; }
		public string OCResourceID { get; set; }
		public string ContentTypeID { get; set; }
	}

	public class JSONContentAssignment : CosmosObject
	{
		public string JSONContentID { get; set; }
		public OCResourceType OCResourceType { get; set; }
		public string OCResourceID { get; set; }
	}

	public class ContentType : CosmosObject
	{
		public string Name { get; set; } // matches ID
		public ????? JSONSchema    // How to do this? Is it worth it?
	}

	// OC Resources that content can be assigned to.
	public enum OCResourceType {
		Product, Category, Catalog, Promotion, Supplier, Buyer, BuyerAddress, SupplierAddrress,
		AdminAddress, BuyerUserGroup, SupplierUserGroup, AdminUserGroup
	}
}
