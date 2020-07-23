using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class SchemaHelper
	{
		// For for details see https://www.newtonsoft.com/jsonschema

		public static DocumentSchemaDO ValidateSchema(DocumentSchemaDO schema)
		{
			IList<string> errors;
			var isValid = schema.Schema.IsValid(JSchema.Parse(SchemaForSchemas.JSON), out errors);
			if (!isValid) throw new SchemaNotValidException(errors);
			return schema;
		}

		public static DocumentDO ValidateDocumentAgainstSchema(DocumentSchemaDO schema, DocumentDO document)
		{
			IList<string> errors;
			var isValid = document.Doc.IsValid(JSchema.Parse(schema.Schema.ToString()), out errors);
			if (!isValid) throw new DocumentNotValidException(schema.InteropID, errors);
			return document;
		}
	}
}
