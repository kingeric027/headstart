using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class DocSchema
	{
		[CosmosInteropID]
		public string ID { get; set; }
		public List<ResourceType> RestrictedAssignmentTypes { get; set; } = new List<ResourceType>(); // empty means no restrictions
		[Required]
		public JObject Schema { get; set; }
		[ApiReadOnly]
		public History History { get; set; }
	}
}
