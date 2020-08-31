using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class DocumentMapper
	{
		public static Document<T> MapTo<T>(DocumentDO doc)
		{
			return new Document<T>()
			{
				ID = doc.InteropID,
				Doc = doc.Doc.ToObject<T>(),
				SchemaSpecUrl = doc.SchemaSpecUrl,
				History = doc.History
			};
		}

		public static DocumentDO MapTo<T>(Document<T> doc)
		{
			return new DocumentDO()
			{
				InteropID = doc.ID,
				Doc = JObject.FromObject(doc.Doc),
				SchemaSpecUrl = doc.SchemaSpecUrl,
				History = doc.History
			};
		}

		public static IEnumerable<Document<T>> MapTo<T>(IEnumerable<DocumentDO> docs)
		{
			return docs.Select(doc => MapTo<T>(doc));
		}

		public static ListPage<Document<T>> MapTo<T>(ListPage<DocumentDO> listPage)
		{
			return new ListPage<Document<T>>
			{
				Meta = listPage.Meta,
				Items = MapTo<T>(listPage.Items).ToList()
			};
		}

		public static ListArgs<DocumentDO> MapTo<T>(this ListArgs<Document<T>> args)
		{
			return args.MapTo<Document<T>, DocumentDO>(new ListArgMap()
			{
				{"ID", "InteropID" }
			});
		}
	}
}
