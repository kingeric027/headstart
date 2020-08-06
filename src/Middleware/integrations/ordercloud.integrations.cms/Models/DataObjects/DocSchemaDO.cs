using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("documentschemas")]
	public class DocSchemaDO : CosmosObject
	{
		public string InteropID { get; set; }
		[CosmosPartitionKey]
		public string SellerOrgID { get; set; }
		public List<ResourceType> RestrictedAssignmentTypes { get; set; } = new List<ResourceType>(); // empty means no restrictions
		public JObject Schema { get; set; }
		public History History { get; set; }
		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID", "/SellerOrgID" }}
			};
		}
	}
}
