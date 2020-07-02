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
		public static readonly string SchemaForSchemas = "http://json-schema.org/draft-07/schema";

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

		public static void ValidateSchema(DocumentSchema schema)
		{
			IList<string> errors;
			var isValid = schema.Schema.IsValid(GetSchemaForSchemas(), out errors);
			if (!isValid) throw new SchemaNotValidException(errors);
		}

		public static void ValidateDocumentAgainstSchema(DocumentSchema schema, Document document)
		{
			IList<string> errors;
			var isValid = schema.Schema.IsValid(Map(schema), out errors);
			if (!isValid) throw new DocumentNotValidException(schema.InteropID, errors);
		}
	}
}
