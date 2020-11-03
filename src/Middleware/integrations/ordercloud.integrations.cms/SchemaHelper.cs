using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JsonSchema = NJsonSchema.JsonSchema;
using System.Linq;
using NJsonSchema.Validation;

namespace ordercloud.integrations.cms
{
	public static class SchemaHelper
	{
		// For for details see https://github.com/RicoSuter/NJsonSchema

		public async static Task<DocSchemaDO> ValidateSchema(DocSchemaDO schema)
		{
			var NJschema = await JsonSchema.FromJsonAsync(SchemaForSchemas.JSON);
			var errors = NJschema.Validate(schema.Schema);
			if (errors.Count > 0) throw new SchemaNotValidException(FormatErrors(errors));
			return schema;
		}

		public async static Task<DocumentDO> ValidateDocumentAgainstSchema(DocSchemaDO schema, DocumentDO document)
		{
			var NJschema = await JsonSchema.FromJsonAsync(schema.Schema.ToString());
			var errors = NJschema.Validate(document.Doc);
			if (errors.Count > 0) throw new DocumentNotValidException(schema.InteropID, FormatErrors(errors));
			return document;
		}

		private static List<string> FormatErrors(IEnumerable<ValidationError> errors)
		{
			return errors.Select(e => $"{e.Path}: {e.Kind}").ToList();
		}
	}
}
