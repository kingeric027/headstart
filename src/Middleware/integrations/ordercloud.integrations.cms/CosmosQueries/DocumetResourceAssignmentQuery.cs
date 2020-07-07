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
					doc.ResourceID == resource.ID &&
					doc.ResourceParentID == resource.ParentID &&
					doc.ResourceType == resource.Type
				);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var assignments = list.ToListPage(args.Page, args.PageSize, count);
			var documents = await Throttler.RunAsync(assignments.Items, 5, 20, assignment => _documents.Get(assignment.DocumentID)); // maybe switch to SQL "IN"
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
			if (!schema.AllowedResourceAssociations.Contains(resource.Type))
			{
				throw new InvalidAssignmentException(); // TODO - Add AllowedResourceAssociations list to error
			}
			var document = await _documents.GetWithSchemaDBID(schema.id, documentInteropID, user);
			await _store.UpsertAsync(new DocumentResourceAssignment()
			{
				ResourceID = resource.ID,
				ResourceParentID = resource.ParentID,
				ResourceType = resource.Type,
				OwnerClientID = user.ClientID,
				SchemaID = schema.id,
				DocumentID = document.id
			});
		}

		public async Task DeleteAssignment(string schemaInteropID, string documentInteropID, Resource resource, VerifiedUserContext user)
		{
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var schema = await _schemas.Get(schemaInteropID, user);
			var document = await _documents.GetWithSchemaDBID(schema.id, documentInteropID, user);
			var assignment = new DocumentResourceAssignment()
			{
				ResourceID = resource.ID,
				ResourceParentID = resource.ParentID,
				ResourceType = resource.Type,
				OwnerClientID = user.ClientID,
				SchemaID = schema.id,
				DocumentID = document.id
			};
			var query = @"select top 1 * from c where 
							c.ResourceType = @ResourceType 
							AND c.ResourceID = @ResourceID 
							AND c.ResourceParentID = @ResourceParentID
							AND c.SchemaID = @SchemaID
							AND c.DocumentID = @DocumentID
							AND c.OwnerClientID = @OwnerClientID";
			assignment = await _store.QuerySingleAsync(query, assignment);
			if (assignment != null) { // TODO - what is correct way to handle delete that doesn't exist?
				await _store.RemoveByIdAsync(assignment.id, user.ClientID);
			}
		}
	}
}
