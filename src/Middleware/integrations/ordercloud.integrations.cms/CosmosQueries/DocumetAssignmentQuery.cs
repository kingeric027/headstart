using Cosmonaut;
using Cosmonaut.Extensions;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ordercloud.integrations.cms
{
	public interface IDocumentAssignmentQuery
	{
		Task<List<Document<T>>> ListDocuments<T>(string schemaInteropID, Resource resource, VerifiedUserContext user);
		Task<ListPage<DocumentAssignment>> ListAssignments(string schemaInteropID, ListArgs<DocumentAssignment> args, VerifiedUserContext user);
		Task SaveAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user);
		Task DeleteAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user);
	}

	public class DocumentAssignmentQuery : IDocumentAssignmentQuery
	{
		private readonly ICosmosStore<DocumentAssignmentDO> _store;
		private readonly IDocumentQuery _documents;
		private readonly ISchemaQuery _schemas;

		public DocumentAssignmentQuery(ICosmosStore<DocumentAssignmentDO> store, IDocumentQuery documents, ISchemaQuery schemas)
		{
			_store = store;
			_documents = documents;
			_schemas = schemas;
		}

		public async Task<List<Document<T>>> ListDocuments<T>(string schemaInteropID, Resource resource, VerifiedUserContext user) 
		{
			// Confirm user has access to resource.
			// await new MultiTenantOCClient(user).Get(resource); Commented out until I solve visiblity for /me endpoints
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var assignments = await _store.Query()
				.Where(doc =>
					doc.SchemaID == schema.id && 
					doc.RsrcID == resource.ResourceID &&
					doc.ParentRsrcID == resource.ParentResourceID &&
					doc.RsrcType== resource.ResourceType
				).ToListAsync();
			var documentIDs = assignments.Select(assign => assign.DocID);
			var documents = DocumentMapper.MapTo<T>(await _documents.ListByInternalIDs(documentIDs));
			return documents.ToList();
		}

		public async Task<ListPage<DocumentAssignment>> ListAssignments(string schemaInteropID, ListArgs<DocumentAssignment> args, VerifiedUserContext user)
		{
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var arguments = args.MapTo();
			var query = _store.Query()
				.Search(arguments)
				.Filter(arguments)
				.Sort(arguments)
				.Where(doc => doc.SchemaID == schema.id);
			var list = await query.WithPagination(arguments.Page, arguments.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var assignments = list.ToListPage(arguments.Page, arguments.PageSize, count);
			var documentIDs = assignments.Items.Select(assign => assign.DocID);
			var documents = await _documents.ListByInternalIDs(documentIDs);
			return DocumentAssignmentMapper.MapTo(assignments, documents);
		}

		public async Task SaveAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user)
		{
			var resource = assignment.MapToResource();
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var schema = await _schemas.GetDO(schemaInteropID, user);
			if (!isValidAssignment(schema.RestrictedAssignmentTypes, resource.ResourceType ?? 0))
			{
				throw new InvalidAssignmentException(schema.RestrictedAssignmentTypes);
			}
			var document = await _documents.GetDOByInternalSchemaID(schema.id, assignment.DocumentID, user);
			await _store.UpsertAsync(new DocumentAssignmentDO()
			{
				RsrcID = assignment.ResourceID,
				ParentRsrcID = assignment.ParentResourceID,
				RsrcType = resource.ResourceType ?? 0,
				SellerOrgID = user.SellerID,
				SchemaID = schema.id,
				DocID = document.id
			});
		}

		public async Task DeleteAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user)
		{
			var resource = assignment.MapToResource();
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var document = await _documents.GetDOByInternalSchemaID(schema.id, assignment.DocumentID, user);
			// TODO - what is the correct way to handle delete that doesn't exist?
			await _store.RemoveAsync(assign =>
				assign.RsrcID == assignment.ResourceID &&
				assign.ParentRsrcID == assignment.ParentResourceID &&
				assign.RsrcType == assignment.ResourceType &&
				assign.SellerOrgID == user.ClientID &&
				assign.SchemaID == schema.id &&
				assign.DocID == document.id
			);
		}

		private bool isValidAssignment(List<ResourceType> restrictedAssignmentTypes, ResourceType thisType)
		{
			return restrictedAssignmentTypes.Count == 0 || restrictedAssignmentTypes.Contains(thisType);
		}
	}
}
