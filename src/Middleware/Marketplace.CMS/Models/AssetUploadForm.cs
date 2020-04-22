using Marketplace.Helpers.Helpers.Attributes;
using Microsoft.AspNetCore.Http;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS.Models
{
	public class AssetUploadForm
	{
		[InteropID]
		public string ID { get; set; }
		public IFormFile File { get; set; }
		public string UrlPathOveride { get; set; }
		public string Title { get; set; } = null;
		public List<string> Tags { get; set; }
		public AssetType? Type { get; set; } = null;
		public string FileName { get; set; }
		public bool Active { get; set; } = false;
	}
}
