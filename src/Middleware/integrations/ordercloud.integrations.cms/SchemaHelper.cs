using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using ordercloud.integrations.cms.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class SchemaHelper
	{
		public static DocumentSchema ValidateSchema(DocumentSchema schema, CMSConfig config)
		{
			IList<string> errors;
			schema = AddSchemaMetaData(schema, config);
			var isValid = schema.Schema.IsValid(JSchema.Parse(SchemaForSchemas.JSON), out errors);
			if (!isValid) throw new SchemaNotValidException(errors);
			return schema;
		}

		public static Document ValidateDocumentAgainstSchema(DocumentSchema schema, Document document, CMSConfig config)
		{
			IList<string> errors;
			var isValid = document.Doc.IsValid(JSchema.Parse(schema.Schema.ToString()), out errors);
			if (!isValid) throw new DocumentNotValidException(schema.InteropID, errors);
			return document;
		}

		private static DocumentSchema AddSchemaMetaData(DocumentSchema schema, CMSConfig config)
		{
			schema.Schema["$schema"] = $"{config.BaseUrl}/schema-specs/metaschema";
			schema.Schema["$id"] = $"{config.BaseUrl}/schema-specs/{schema.id}";
			schema.Schema["title"] = schema.Title;
			return schema;
		}
	}
}
