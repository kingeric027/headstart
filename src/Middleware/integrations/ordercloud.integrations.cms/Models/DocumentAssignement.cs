using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class DocumentAssignment
	{
		[Required]
		public string ResourceID { get; set; }
		[Required]
		public ResourceType? ResourceType { get; set; } // nullable so the required validation works
		public string ParentResourceID { get; set; } = null;
		[Required]
		public string DocumentID { get; set; }
	}
}
