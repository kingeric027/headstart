using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ordercloud.integrations.cms
{
	public class DocumentResourceAssignment : CosmosObject
	{
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; } = null;
		public ResourceType ResourceType { get; set; }

		[CosmosPartitionKey, ApiIgnore]
		public string OwnerClientID { get; set; }
		public string SchemaID { get; set; } // real ID
		public string DocumentID { get; set; } // real ID

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/Resource/ID", "/Resource/Type", "/Resource/ParentID" }}
			};
		}
	}
}
