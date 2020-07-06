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
		public static DocumentSchema ValidateSchema(DocumentSchema schema)
		{
			IList<string> errors;
			var isValid = schema.Schema.IsValid(JSchema.Parse(SchemaForSchemas.JSON), out errors);
			if (!isValid) throw new SchemaNotValidException(errors);
			return schema;
		}

		public static Document ValidateDocumentAgainstSchema(DocumentSchema schema, Document document)
		{
			IList<string> errors;
			var isValid = document.Doc.IsValid(JSchema.Parse(schema.Schema.ToString()), out errors);
			if (!isValid) throw new DocumentNotValidException(schema.InteropID, errors);
			return document;
		}
	}
}
