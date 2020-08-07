using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
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
		[CosmosInteropID]
		public string ResourceID { get; set; }
		[Required]
		public ResourceType? ResourceType { get; set; } // nullable so the required validation works
		[RequireBasedOnType]
		public string ParentResourceID { get; set; } = null;		
		[Required]
		public string DocumentID { get; set; }  // NOTE - Cannot filter on DocumentID
	}
}
