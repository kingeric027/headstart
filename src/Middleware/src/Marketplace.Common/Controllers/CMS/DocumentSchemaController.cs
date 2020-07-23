using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers.CMS
{
	[DocComments("\"Integration\" represents Document Schemas")]
	[MarketplaceSection.Content(ListOrder = 2)]
	[Route("schemas")]
	public class DocumentSchemaController : BaseController
	{
		private readonly IDocumentSchemaQuery _schemas;

		public DocumentSchemaController(AppSettings settings, IDocumentSchemaQuery schemas) : base(settings)
		{
			_schemas = schemas;
		}

		[DocName("List Document Schemas")]
		[HttpGet, Route(""), OrderCloudIntegrationsAuth]
		public async Task<ListPage<DocumentSchemaDO>> List(ListArgs<DocumentSchemaDO> args)
		{
			return await _schemas.List(args, VerifiedUserContext);
		}

		[DocName("Get a Document Schema")]
		[HttpGet, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task<DocumentSchemaDO> Get(string schemaID)
		{
			return await _schemas.Get(schemaID, VerifiedUserContext);
		}

		[DocName("Create a Document Schema")]
		[DocIgnore]
		[HttpPost, Route(""), OrderCloudIntegrationsAuth]
		public async Task<DocumentSchemaDO> Create([FromBody] DocumentSchemaDO schema)
		{
			return await _schemas.Create(schema, VerifiedUserContext);
		}

		[DocName("Update a Document Schema")]
		[HttpPut, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task<DocumentSchemaDO> Update(string schemaID, [FromBody] DocumentSchemaDO schema)
		{
			return await _schemas.Update(schemaID, schema, VerifiedUserContext);
		}

		[DocName("Delete a Document Schema")]
		[HttpDelete, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task Delete(string schemaID)
		{
			await _schemas.Delete(schemaID, VerifiedUserContext);
		}
	}

	public class DocumentSchemaSpecController : BaseController
	{
		private readonly IDocumentSchemaQuery _schemas;

		public DocumentSchemaSpecController(AppSettings settings, IDocumentSchemaQuery schemas) : base(settings)
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
