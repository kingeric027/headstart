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
using Marketplace.Models.Misc;

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
		public async Task<ListPage<JDocument>> List(string schemaID, ListArgs<Document<JObject>> args)
		{
			RequireOneOf(CustomRole.DocumentAdmin, CustomRole.DocumentReader);
			var docs = await _documents.List<JObject>(schemaID, args, VerifiedUserContext);
			return docs.Reserialize<ListPage<JDocument>>();
		}

		[DocName("Get a Document")]
		[HttpGet, Route("{documentID}"), OrderCloudIntegrationsAuth]
		public async Task<JDocument> Get(string schemaID, string documentID)
		{
			RequireOneOf(CustomRole.DocumentAdmin, CustomRole.DocumentReader);
			var doc = await _documents.Get<JObject>(schemaID, documentID, VerifiedUserContext);
			return doc.Reserialize<JDocument>();
		}

		[DocName("Create a Document")]
		[HttpPost, Route(""), OrderCloudIntegrationsAuth]
		public async Task<JDocument> Create(string schemaID, [FromBody] JDocument document)
		{
			RequireOneOf(CustomRole.DocumentAdmin);
			var doc = await _documents.Create(schemaID, document, VerifiedUserContext);
			return doc.Reserialize<JDocument>();
		}

		[DocName("Update a Document")]
		[HttpPut, Route("{documentID}"), OrderCloudIntegrationsAuth]
		public async Task<JDocument> Save(string schemaID, string documentID, [FromBody] JDocument document)
		{
			RequireOneOf(CustomRole.DocumentAdmin);
			var doc =  await _documents.Save(schemaID, documentID, document, VerifiedUserContext);
			return doc.Reserialize<JDocument>();
		}

		[DocName("Delete a Document")]
		[HttpDelete, Route("{documentID}"), OrderCloudIntegrationsAuth]
		public async Task Delete(string schemaID, string documentID)
		{
			RequireOneOf(CustomRole.DocumentAdmin);
			await _documents.Delete(schemaID, documentID, VerifiedUserContext);
		}

		[DocName("List Document Assignments")]
		[HttpGet, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task<ListPage<DocumentAssignment>> ListAssignments(string schemaID, ListArgs<DocumentAssignment> args)
		{
			RequireOneOf(CustomRole.DocumentReader, CustomRole.DocumentAdmin);
			return await _assignments.ListAssignments(schemaID, args, VerifiedUserContext);
		}

		[DocName("Create Document Assignment")]
		[HttpPost, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task SaveAssignment(string schemaID, [FromBody] DocumentAssignment assignment)
		{
			RequireOneOf(CustomRole.DocumentAdmin);
			await _assignments.SaveAssignment(schemaID, assignment, VerifiedUserContext);
		}

		[DocName("Delete Document Assignment")]
		[HttpDelete, Route("assignments"), OrderCloudIntegrationsAuth]
		public async Task DeleteAssignment(string schemaID, [FromQuery] DocumentAssignment assignment)
		{
			RequireOneOf(CustomRole.DocumentAdmin);
			await _assignments.DeleteAssignment(schemaID, assignment, VerifiedUserContext);
		}

		[DocName("List Documents Assigned to Resource"), OrderCloudIntegrationsAuth]
		[HttpGet, Route("{type}/{ID}")]
		public async Task<ListPage<JDocument>> ListDocuments(string schemaID, ResourceType type, string ID, [FromQuery] ListArgsPageOnly args)
		{
			var resource = new Resource(type, ID);
			var docs = await _assignments.ListDocuments<JObject>(schemaID, resource, args, VerifiedUserContext);
			return docs.Reserialize<ListPage<JDocument>>();
		}

		[DocName("List Documents Assigned to Resource"), OrderCloudIntegrationsAuth]
		[HttpGet, Route("{parentType}/{parentID}/{type}/{ID}")]
		public async Task<ListPage<JDocument>> ListDocumentsOnChild(string schemaID, ParentResourceType parentType, string parentID, ResourceType type, string ID, [FromQuery] ListArgsPageOnly args)
		{
			var resource = new Resource(type, ID, parentType, parentID);
			var docs = await _assignments.ListDocuments<JObject>(schemaID, resource, args, VerifiedUserContext);
			return docs.Reserialize<ListPage<JDocument>>();
		}
	}
}
