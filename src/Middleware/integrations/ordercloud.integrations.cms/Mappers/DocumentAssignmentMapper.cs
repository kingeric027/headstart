using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class DocumentAssignmentMapper
	{
		public static DocumentAssignment MapTo(DocumentAssignmentDO assignment, DocumentDO doc)
		{
			return new DocumentAssignment()
			{
				ResourceID = assignment.RsrcID,
				ResourceType = assignment.RsrcType,
				ParentResourceID = assignment.ParentRsrcID,
				ParentResourceType = assignment.RsrcType.GetParentType(),
				DocumentID = doc.InteropID
			};
		}

		public static IEnumerable<DocumentAssignment> MapTo(IEnumerable<DocumentAssignmentDO> assignments, IEnumerable<DocumentDO> docs)
		{
			var docLookup = docs.ToDictionary(doc => doc.id);
			return assignments.Select(assignment => MapTo(assignment, docLookup[assignment.DocID]));
		}

		public static ListPage<DocumentAssignment> MapTo(ListPage<DocumentAssignmentDO> listPage, IEnumerable<DocumentDO> docs)
		{
			return new ListPage<DocumentAssignment>
			{
				Meta = listPage.Meta,
				Items = MapTo(listPage.Items, docs).ToList()
			};
		}

		public static ListArgs<DocumentAssignmentDO> MapTo(this ListArgs<DocumentAssignment> args)
		{
			return args.MapTo<DocumentAssignment, DocumentAssignmentDO>(new Dictionary<string, string>()
			{
				{"ResourceID", "RsrcID" },
				{"ParentResourceID", "ParentRsrcID" },
				{"ResourceType", "RsrcType" },
			});
		}
	}
}
