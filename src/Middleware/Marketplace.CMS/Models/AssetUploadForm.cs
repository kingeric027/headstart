using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS.Models
{
	public class AssetUploadForm
	{
		public IFormFile File { get; set; }
		public string ID { get; set; }
		public string UrlPathOveride { get; set; }
		public string Title { get; set; }
		public List<string> Tags { get; set; }
		public AssetType Type { get; set; }
		public string FileName { get; set; }
	}
}
