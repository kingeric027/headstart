using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers.CMS
{
	[DocComments("\"Integration\" represents Document Schemas")]
	[MarketplaceSection.Content(ListOrder = 2)]
	[Route("schemas")]
	public class SchemaController : BaseController
	{
		private readonly ISchemaQuery _schemas;

		public SchemaController(AppSettings settings, ISchemaQuery schemas) : base(settings)
		{
			_schemas = schemas;
		}

		[DocName("List Document Schemas")]
		[HttpGet, Route(""), OrderCloudIntegrationsAuth]
		public async Task<ListPage<DocSchema>> List(ListArgs<DocSchema> args)
		{
			RequireOneOf(CustomRole.SchemaAdmin, CustomRole.SchemaReader);
			return await _schemas.List(args, VerifiedUserContext);
		}

		[DocName("Get a Document Schema")]
		[HttpGet, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task<DocSchema> Get(string schemaID)
		{
			RequireOneOf(CustomRole.SchemaAdmin, CustomRole.SchemaReader);
			return await _schemas.Get(schemaID, VerifiedUserContext);
		}

		[DocName("Create a Document Schema")]
		[HttpPost, Route(""), OrderCloudIntegrationsAuth]
		public async Task<DocSchema> Create([FromBody] DocSchema schema)
		{
			RequireOneOf(CustomRole.SchemaAdmin);
			return await _schemas.Create(schema, VerifiedUserContext);
		}

		[DocName("Update a Document Schema")]
		[HttpPut, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task<DocSchema> Save(string schemaID, [FromBody] DocSchema schema)
		{
			RequireOneOf(CustomRole.SchemaAdmin);
			return await _schemas.Save(schemaID, schema, VerifiedUserContext);
		}

		[DocName("Delete a Document Schema")]
		[HttpDelete, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task Delete(string schemaID)
		{
			RequireOneOf(CustomRole.SchemaAdmin);
			await _schemas.Delete(schemaID, VerifiedUserContext);
		}
	}

	public class DocumentSchemaSpecController : BaseController
	{
		private readonly ISchemaQuery _schemas;

		public DocumentSchemaSpecController(AppSettings settings, ISchemaQuery schemas) : base(settings)
		{
			_schemas = schemas;
		}

		[DocIgnore]
		[HttpGet, Route("schema-specs/{schemaID}")] // No auth is intentional. Make these available to anyone.
		public async Task<JObject> GetSchemaSpec(string schemaID) // Not interopID!! real, id.
		{
			if (schemaID == "metaschema")
			{
				return JObject.Parse(SchemaForSchemas.JSON);
			}
			var schema = await _schemas.GetByInternalID(schemaID); 
			return schema.Schema;
		}
	}
}
