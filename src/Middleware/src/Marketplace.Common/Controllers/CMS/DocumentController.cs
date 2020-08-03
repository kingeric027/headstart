using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Linq;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ordercloud.integrations.cms;
using IDocumentQuery = ordercloud.integrations.cms.IDocumentQuery;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Controllers.CMS
{
	[DocComments("\"Integration\" represents Documents")]
	[MarketplaceSection.Content(ListOrder = 3)]
	[Route("schemas/{schemaID}/documents")]
	public class DocumentController : BaseController
	{
		private readonly IDocumentQuery _documents;
		private readonly IDocumentAssignmentQuery _assignments;

		public DocumentController(AppSettings settings, IDocumentQuery schemas, IDocumentAssignmentQuery assignments) : base(settings)
		{
			_documents = schemas;
			_assignments = assignments;
		}

		[DocName("List Documents")]
		[HttpGet, Route(""), OrderCloudIntegrationsAuth]
		public async Task<ListPage<Document<JObject>>> List(string schemaID, ListArgs<Document<JObject>> args)
		{
			return await _documents.List<JObject>(schemaID, args, VerifiedUserContext);
		}

		[DocName("Get a Document")]
		[HttpGet, Route("{documentID}"), OrderCloudIntegrationsAuth]
		public async Task<Document<JObject>> Get(string schemaID, string documentID)
		{
			return await _documents.Get<JObject>(schemaID, documentID, VerifiedUserContext);
		}

		[DocName("Create a Document")]
		[HttpPost, Route(""), OrderCloudIntegrationsAuth]
		public async Task<Document<JObject>> Create(string schemaID, [FromBody] Document<JObject> document)
		{
			return await _documents.Create(schemaID, document, VerifiedUserContext);
		}

		[DocName("Update a Document")]
		[HttpPut, Route("{documentID}"), OrderCloudIntegrationsAuth]
		public async Task<Document<JObject>> Update(string schemaID, string documentID, [FromBody] Document<JObject> document)
		{
			return await _documents.Update(schemaID, documentID, document, VerifiedUserContext);
		}

		[DocName("Delete a Document")]
		[HttpDelete, Route("{documentID}"), OrderCloudIntegrationsAuth]
		public async Task Delete(string schemaID, string documentID)
		{
			await _documents.Delete<JObject>(schemaID, documentID, VerifiedUserContext);
		}

		[DocName("List Document Assignments")]
		[HttpGet, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task<ListPage<DocumentAssignment>> ListAssignments(string schemaID, ListArgs<DocumentAssignment> args)
		{
			return await _assignments.ListAssignments(schemaID, args, VerifiedUserContext);
		}

		[DocName("Create Document Assignment")]
		[HttpPost, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task SaveAssignment(string schemaID, [FromBody] DocumentAssignment assignment)
		{
			await _assignments.SaveAssignment(schemaID, assignment, VerifiedUserContext);
		}

		[DocName("Delete Document Assignment")]
		[HttpDelete, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task DeleteAssignment(string schemaID, [FromQuery] DocumentAssignment assignment)
		{
			await _assignments.DeleteAssignment(schemaID, assignment, VerifiedUserContext);
		}

		[DocName("List Documents Assigned to Resource"), OrderCloudIntegrationsAuth]
		[HttpGet, Route("resource")]
		public async Task<List<Document>> ListDocuments(string schemaID, [FromQuery] Resource resource)
		{
			return await _assignments.ListDocuments(schemaID, resource, VerifiedUserContext);
		}
	}
}
