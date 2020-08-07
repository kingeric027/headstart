using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class SchemaMapper
	{
		public static DocSchema MapTo(DocSchemaDO schema)
		{
			return new DocSchema()
			{
				ID = schema.InteropID,
				RestrictedAssignmentTypes = schema.RestrictedAssignmentTypes,
				Schema = schema.Schema,
				History = schema.History

			};
		}

		public static DocSchemaDO MapTo(DocSchema schema)
		{
			return new DocSchemaDO()
			{
				InteropID = schema.ID,
				RestrictedAssignmentTypes = schema.RestrictedAssignmentTypes,
				Schema = schema.Schema,
				History = schema.History,
			};
		}

		public static IEnumerable<DocSchema> MapTo(IEnumerable<DocSchemaDO> schemas)
		{
			return schemas.Select(schema => MapTo(schema));
		}

		public static ListPage<DocSchema> MapTo(ListPage<DocSchemaDO> listPage)
		{
			return new ListPage<DocSchema>
			{
				Meta = listPage.Meta,
				Items = MapTo(listPage.Items).ToList()
			};
		}
	}
}
