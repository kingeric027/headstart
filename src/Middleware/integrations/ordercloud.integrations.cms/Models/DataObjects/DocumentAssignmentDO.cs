using Cosmonaut.Attributes;
using Microsoft.Azure.Documents;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ordercloud.integrations.cms
{
	[CosmosCollection("documentassignments")]
	public class DocumentAssignmentDO : CosmosObject
	{
		public string RsrcID { get; set; }
		public string ParentRsrcID { get; set; } = null;
		public ResourceType RsrcType { get; set; }
		[CosmosPartitionKey]
		public string SellerOrgID { get; set; }
		public string SchemaID { get; set; } // real ID
		public string DocID { get; set; } // real ID

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				// Resource is abreviated as Rsrc because I'm running up against a 60 char limit in Cosmos for the sum of field paths in a unique key
				new UniqueKey() { Paths = new Collection<string> { "/RsrcID", "/RsrcType", "/ParentRsrcID", "/SellerOrgID", "/SchemaID", "/DocID"  }}
			};
		}
	}
}
