using Cosmonaut.Attributes;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using ordercloud.integrations.library.Cosmos;
using System.Collections.ObjectModel;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ordercloud.integrations.cms.Models
{
	[SwaggerModel]
	public class Document
	{
		[Required]
		public string ID { get; set; }
		[Required]
		public JObject Doc { get; set; }
		[ApiReadOnly]
		public string SchemaSpecUrl { get; set; }
		[ApiReadOnly]
		public History History { get; set; }
	}

	[SwaggerModel]
	public class Document<TDoc> : Document
	{
		public new TDoc Doc { get; set; }

	}

}
