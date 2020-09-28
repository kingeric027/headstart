using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using Newtonsoft.Json;
using ordercloud.integrations.library.Cosmos;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class JDocument : Document<JObject> { }

	public class Document<T>
	{
		[CosmosInteropID]
		public string ID { get; set; }
		[Required]
		public T Doc { get; set; }
		[ApiReadOnly]
		public string SchemaSpecUrl { get; set; }
		[ApiReadOnly]
		public History History { get; set; }
	}
}
