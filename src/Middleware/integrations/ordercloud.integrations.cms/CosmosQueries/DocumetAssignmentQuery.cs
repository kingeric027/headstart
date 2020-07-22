using Cosmonaut;
using Cosmonaut.Extensions;
using ordercloud.integrations.cms.Mappers;
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
		Task<ListPage<Document>> ListDocuments(string schemaInteropID, Resource resource, ListArgs<DocumentAssignment> args, VerifiedUserContext user);
		Task<ListPage<DocumentAssignment>> ListAssignments(string schemaInteropID, ListArgs<DocumentAssignment> args, VerifiedUserContext user);
		Task SaveAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user);
		Task DeleteAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user);
	}

	public class DocumentAssignmentQuery : IDocumentAssignmentQuery
	{
		private readonly ICosmosStore<DocumentAssignmentDO> _store;
		private readonly IDocumentQuery _documents;
		private readonly IDocumentSchemaQuery _schemas;

		public DocumentAssignmentQuery(ICosmosStore<DocumentAssignmentDO> store, IDocumentQuery documents, IDocumentSchemaQuery schemas)
		{
			_store = store;
			_documents = documents;
			_schemas = schemas;
		}

		public async Task<ListPage<Document>> ListDocuments(string schemaInteropID, Resource resource, ListArgs<DocumentAssignment> args, VerifiedUserContext user) 
		{
			// Confirm user has access to resource.
			// await new MultiTenantOCClient(user).Get(resource); Commented out until I solve visiblity for /me endpoints
			var schema = await _schemas.Get(schemaInteropID, user);
			var query = _store.Query()
				.Search(args)
				.Filter(args)
				.Sort(args)
				.Where(doc => 
					doc.SchemaID == schema.id && 
					doc.RsrcID == resource.ID &&
					doc.ParentRsrcID == resource.ParentID &&
					doc.RsrcType== resource.Type
				);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var assignments = list.ToListPage(args.Page, args.PageSize, count);
			var documentIDs = assignments.Items.Select(assign => assign.DocID);
			var documents = await _documents.ListByInternalIDs(documentIDs);
			return new ListPage<Document>()
			{
				Items = documents,
				Meta = assignments.Meta
			};
		}

		public async Task<ListPage<DocumentAssignment>> ListAssignments(string schemaInteropID, ListArgs<DocumentAssignment> args, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var arguments = args.MapTo();
			var query = _store.Query()
				.Search(arguments)
				.Filter(arguments)
				.Sort(arguments)
				.Where(doc => doc.SchemaID == schema.id);
			var list = await query.WithPagination(arguments.Page, arguments.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var assignments = list.ToListPage(arguments.Page, arguments.PageSize, count);
			return DocumentAssignmentMapper.MapTo(assignments);
		}

		public async Task SaveAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user)
		{
			var resourceType = assignment.ResourceType ?? 0; // "Required" validation should prevent null ResourceType
			var resource = new Resource(resourceType, assignment.ResourceID, assignment.ParentResourceID);
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var schema = await _schemas.Get(schemaInteropID, user);
			if (!isValidAssignment(schema.RestrictedAssignmentTypes, resourceType))
			{
				throw new InvalidAssignmentException(schema.RestrictedAssignmentTypes);
			}
			var document = await _documents.GetByInternalSchemaID(schema.id, assignment.DocumentID, user);
			await _store.UpsertAsync(new DocumentAssignmentDO()
			{
				RsrcID = assignment.ResourceID,
				ParentRsrcID = assignment.ParentResourceID,
				RsrcType = resourceType,
				ClientID = user.ClientID,
				SchemaID = schema.id,
				DocID = document.id
			});
		}

		public async Task DeleteAssignment(string schemaInteropID, DocumentAssignment assignment, VerifiedUserContext user)
		{
			var resourceType = assignment.ResourceType ?? 0; // "Required" validation should prevent null ResourceType
			var resource = new Resource(resourceType, assignment.ResourceID, assignment.ParentResourceID);
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var schema = await _schemas.Get(schemaInteropID, user);
			var document = await _documents.GetByInternalSchemaID(schema.id, assignment.DocumentID, user);
			// TODO - what is the correct way to handle delete that doesn't exist?
			await _store.RemoveAsync(assign =>
				assign.RsrcID == assignment.ResourceID &&
				assign.ParentRsrcID == assignment.ParentResourceID &&
				assign.RsrcType == assignment.ResourceType &&
				assign.ClientID == user.ClientID &&
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
