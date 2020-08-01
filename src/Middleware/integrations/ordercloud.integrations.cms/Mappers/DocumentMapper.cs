using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class DocumentMapper
	{
		public static Document MapTo(DocumentDO doc)
		{
			return new Document()
			{
				ID = doc.InteropID,
				Doc = doc.Doc,
				SchemaSpecUrl = doc.SchemaSpecUrl,
				History = doc.History
			};
		}

		public static DocumentDO MapTo(Document doc)
		{
			return new DocumentDO()
			{
				InteropID = doc.ID,
				Doc = doc.Doc,
				SchemaSpecUrl = doc.SchemaSpecUrl,
				History = doc.History
			};
		}

		public static IEnumerable<Document> MapTo(IEnumerable<DocumentDO> docs)
		{
			return docs.Select(doc => MapTo(doc));
		}

		public static ListPage<Document> MapTo(ListPage<DocumentDO> listPage)
		{
			return new ListPage<Document>
			{
				Meta = listPage.Meta,
				Items = MapTo(listPage.Items).ToList()
			};
		}
	}
}
