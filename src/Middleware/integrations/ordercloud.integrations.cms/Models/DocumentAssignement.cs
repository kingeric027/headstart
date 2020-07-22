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

	[CosmosCollection("documentassignments")]
	public class DocumentAssignmentDO : CosmosObject
	{
		public string RsrcID { get; set; }   
		public string ParentRsrcID { get; set; } = null;
		public ResourceType RsrcType { get; set; }
		[CosmosPartitionKey]
		public string ClientID { get; set; }
		public string SchemaID { get; set; } // real ID
		public string DocID { get; set; } // real ID

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				// Resource is abreviated as Rsrc because I'm running up against a 60 char limit in Cosmos for the sum of field paths in a unique key
				new UniqueKey() { Paths = new Collection<string> { "/RsrcID", "/RsrcType", "/ParentRsrcID", "/ClientID", "/SchemaID", "/DocID"  }}
			};
		}
	}
}
