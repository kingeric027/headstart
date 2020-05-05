using Marketplace.CMS.Models;
using Marketplace.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Integrations.CMS.Models
{
	[SwaggerModel]
	public class AssetAssignment
	{
		[Required]
		public ResourceType ResourceType { get; set; }
		[Required]
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; }
		[Required]
		public string AssetID { get; set; }
	}
}
