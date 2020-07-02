using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using ordercloud.integrations.cms.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class SchemaHelper
	{
		private static readonly string SchemaForSchemas = "http://json-schema.org/draft-07/schema";

		public static DocumentSchema ValidateSchema(DocumentSchema schema, CMSConfig config)
		{
			IList<string> errors;
			schema = AddSchemaMetaData(schema, config);
			var isValid = schema.Schema.IsValid(GetSchemaForSchemas(), out errors);
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
			schema.Schema["$schema"] = SchemaForSchemas;
			schema.Schema["$id"] = SchemaSpecUrl(schema, config);
			schema.Schema["title"] = schema.Title;
			return schema;
		}

		private static Document AddDocumentMetaData(DocumentSchema schema, Document document, CMSConfig config)
		{
			document.SchemaSpecUrl = SchemaSpecUrl(schema, config);
			return document;
		}

		private static JSchema GetSchemaForSchemas()
		{
			var schema = JSchema.Parse(new JObject(new JProperty("$ref", SchemaForSchemas)).ToString(), new JSchemaUrlResolver());
			schema.AllowAdditionalProperties = false;
			return schema;
		}

		private static JSchema Map(DocumentSchema schema)
		{
			return JSchema.Parse(schema.Schema.ToString());
		}

		private static string SchemaSpecUrl(DocumentSchema schema, CMSConfig config) =>
			$"{config.BaseUrl}/schemas/spec/{schema.SellerOrgID}/{schema.InteropID}";
	}
}
