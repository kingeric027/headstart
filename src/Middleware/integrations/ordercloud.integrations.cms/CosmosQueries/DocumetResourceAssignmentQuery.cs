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
	public interface IDocumentResourceAssignmentQuery
	{
		Task<ListPage<Document>> ListDocuments(string schemaInteropID, Resource resource, IListArgs args, VerifiedUserContext user);
		Task SaveAssignment(string schemaInteropID, string documentInteropID, Resource resource, VerifiedUserContext user);
		Task DeleteAssignment(string schemaInteropID, string documentInteropID, Resource resource, VerifiedUserContext user);
	}

	public class DocumentResourceAssignmentQuery : IDocumentResourceAssignmentQuery
	{
		private readonly ICosmosStore<DocumentResourceAssignment> _store;
		private readonly IDocumentQuery _documents;
		private readonly IDocumentSchemaQuery _schemas;


		public DocumentResourceAssignmentQuery(ICosmosStore<DocumentResourceAssignment> store, IDocumentQuery documents, IDocumentSchemaQuery schemas)
		{
			_store = store;
			_documents = documents;
			_schemas = schemas;
		}

		public async Task<ListPage<Document>> ListDocuments(string schemaInteropID, Resource resource, IListArgs args, VerifiedUserContext user) 
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
					doc.RsrcParentID == resource.ParentID &&
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

		public async Task SaveAssignment(string schemaInteropID, string documentInteropID, Resource resource, VerifiedUserContext user)
		{
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var schema = await _schemas.Get(schemaInteropID, user);
			if (!isValidAssignment(schema.RestrictedAssignmentTypes, resource.Type))
			{
				throw new InvalidAssignmentException(schema.RestrictedAssignmentTypes);
			}
			var document = await _documents.GetByInternalSchemaID(schema.id, documentInteropID, user);
			await _store.UpsertAsync(new DocumentResourceAssignment()
			{
				RsrcID = resource.ID,
				RsrcParentID = resource.ParentID,
				RsrcType = resource.Type,
				ClientID = user.ClientID,
				SchemaID = schema.id,
				DocID = document.id
			});
		}

		private bool isValidAssignment(List<ResourceType> restrictedAssignmentTypes, ResourceType thisType)
		{
			return restrictedAssignmentTypes.Count == 0 || restrictedAssignmentTypes.Contains(thisType);
		}

		public async Task DeleteAssignment(string schemaInteropID, string documentInteropID, Resource resource, VerifiedUserContext user)
		{
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var schema = await _schemas.Get(schemaInteropID, user);
			var document = await _documents.GetByInternalSchemaID(schema.id, documentInteropID, user);
			// TODO - what is the correct way to handle delete that doesn't exist?
			await _store.RemoveAsync(assign =>
				assign.RsrcID == resource.ID &&
				assign.RsrcParentID == resource.ParentID &&
				assign.RsrcType == resource.Type &&
				assign.ClientID == user.ClientID &&
				assign.SchemaID == schema.id &&
				assign.DocID == document.id
			);
		}
	}
}
