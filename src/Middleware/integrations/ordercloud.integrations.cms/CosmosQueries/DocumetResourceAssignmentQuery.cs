using Cosmonaut;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ordercloud.integrations.cms.CosmosQueries
{
	public interface IDocumentResourceAssignmentQuery
	{
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
							c.ResourceType = @Type 
							AND c.ResourceID = @ID 
							AND c.ResourceParentID = @ParentID
							AND c.SchemaID = @SchemaID
							AND c.DocumentID = @DocumentID
							AND c.OwnerClientID = @OwnerClientID";
			assignment = await _store.QuerySingleAsync(query, assignment);
			await _store.RemoveByIdAsync(assignment.id, user.ClientID);
		}
	}
}
