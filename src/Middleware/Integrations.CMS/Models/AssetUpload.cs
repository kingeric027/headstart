using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Microsoft.AspNetCore.Http;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS.Models
{
	// TODO - is there a way to get rid of this separate class and use normal asset?
	// Because right now we don't have a way to make any of these feilds required. [Required] doesn't seem to work with forms.
	public class AssetUpload
	{
		[InteropID]
		public string ID { get; set; }
		public string Title { get; set; }
		public bool Active { get; set; } = false;
		public IFormFile File { get; set; }
		public string Url { get; set; }
		public AssetType? Type { get; set; } = null;
		public string Tags { get; set; }
		public string FileName { get; set; }
	}
}
