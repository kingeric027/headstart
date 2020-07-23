using ordercloud.integrations.cms.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.cms.Mappers
{
	public static class DocumentAssignmentMapper
	{
		public static DocumentAssignment MapTo(DocumentAssignmentDO assignment)
		{
			return new DocumentAssignment()
			{
				ResourceID = assignment.RsrcID,
				ResourceType = assignment.RsrcType,
				ParentResourceID = assignment.ParentRsrcID,
				DocumentID = assignment.DocID
			};
		}

		public static Resource MapToResource(this DocumentAssignment assignment)
		{
			var resourceType = assignment.ResourceType ?? 0; // "Required" validation should prevent null ResourceType
			return new Resource(resourceType, assignment.ResourceID, assignment.ParentResourceID);
		}

		public static Resource MapToResource(this AssetAssignment assignment)
		{
			var resourceType = assignment.ResourceType ?? 0; // "Required" validation should prevent null ResourceType
			return new Resource(resourceType, assignment.ResourceID, assignment.ParentResourceID);
		}

		public static IEnumerable<DocumentAssignment> MapTo(IEnumerable<DocumentAssignmentDO> assignments)
		{
			return assignments.Select(assignment => MapTo(assignment));
		}

		public static ListPage<DocumentAssignment> MapTo(ListPage<DocumentAssignmentDO> listPage)
		{
			return new ListPage<DocumentAssignment>
			{
				Meta = listPage.Meta,
				Items = MapTo(listPage.Items).ToList()
			};
		}

		public static ListArgs<DocumentAssignmentDO> MapTo(this ListArgs<DocumentAssignment> args)
		{
			return args.MapTo<DocumentAssignment, DocumentAssignmentDO>(new Dictionary<string, string>()
			{
				{"ResourceID", "RsrcID" },
				{"ParentResourceID", "ParentRsrcID" },
				{"ResourceType", "RsrcType" },
				{"DocumentID", "DocID" },
			});
		}
	}
}
