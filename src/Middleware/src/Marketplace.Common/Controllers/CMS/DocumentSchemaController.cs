using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.cms.CosmosQueries;
using ordercloud.integrations.cms.Models;
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
		public async Task<ListPage<DocumentSchema>> List(ListArgs<DocumentSchema> args)
		{
			return await _schemas.List(args, VerifiedUserContext);
		}

		[DocName("Get a Document Schema")]
		[HttpGet, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task<DocumentSchema> Get(string schemaID)
		{
			return await _schemas.Get(schemaID, VerifiedUserContext);
		}

		[DocName("Create a Document Schema")]
		[DocIgnore] // For now, hide from swagger reflection b/c it doesn't handle file uploads well. 
		[HttpPost, Route(""), OrderCloudIntegrationsAuth]
		public async Task<DocumentSchema> Create([FromBody] DocumentSchema schema)
		{
			return await _schemas.Create(schema, VerifiedUserContext);
		}

		[DocName("Update a Document Schema")]
		[HttpPut, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task<DocumentSchema> Update(string schemaID, [FromBody] DocumentSchema schema)
		{
			return await _schemas.Update(schemaID, schema, VerifiedUserContext);
		}

		[DocName("Delete a Document Schema")]
		[HttpDelete, Route("{schemaID}"), OrderCloudIntegrationsAuth]
		public async Task Delete(string schemaID)
		{
			await _schemas.Delete(schemaID, VerifiedUserContext);
		}

		[DocName("Get a formal Document Schema Specification")]
		[HttpGet, Route("spec/{sellerOrgID}/{schemaID}")] // No auth is intentional. Make these available to anyone.
		public async Task<JObject> GetSpecification(string sellerOrgID, string schemaID)
		{
			var schema = await _schemas.Get(schemaID, VerifiedUserContext);
			return schema.Schema;
		}
	}
}
