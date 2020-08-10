using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class AssetAssignment
	{
		[Required]
		public string ResourceID { get; set; }
		[Required]
		public ResourceType? ResourceType { get; set; } // nullable so the required validation works
		[RequireBasedOnResourceType]
		public string ParentResourceID { get; set; } = null;
		[Required]
		public string AssetID { get; set; }
	}
}
