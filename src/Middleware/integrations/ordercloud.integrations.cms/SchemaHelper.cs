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
			var schemaForSchemas = JSchema.Parse(SchemaForSchemas.JSON);
			var isValid = schema.Schema.IsValid(schemaForSchemas, out errors);
			if (!isValid) throw new SchemaNotValidException(errors);
			return schema;
		}

		public static Document ValidateDocumentAgainstSchema(DocumentSchema schema, Document document, CMSConfig config)
		{
			IList<string> errors;
			document = AddDocumentMetaData(schema, document, config);
			var isValid = schema.Schema.IsValid(Map(schema), out errors);
			if (!isValid) throw new DocumentNotValidException(schema.InteropID, errors);
			return document;
		}

		private static DocumentSchema AddSchemaMetaData(DocumentSchema schema, CMSConfig config)
		{
			schema.Schema["$schema"] = $"{config.BaseUrl}/schema-specs/metaschema";
			schema.Schema["$id"] = SchemaSpecUrl(schema, config);
			schema.Schema["title"] = schema.Title;
			return schema;
		}

		private static Document AddDocumentMetaData(DocumentSchema schema, Document document, CMSConfig config)
		{
			document.SchemaSpecUrl = SchemaSpecUrl(schema, config);
			return document;
		}

		private static JSchema Map(DocumentSchema schema)
		{
			return JSchema.Parse(schema.Schema.ToString());
		}

		private static string SchemaSpecUrl(DocumentSchema schema, CMSConfig config) =>
			$"{config.BaseUrl}/schema-specs/{schema.SellerOrgID}/{schema.InteropID}";
	}
}
