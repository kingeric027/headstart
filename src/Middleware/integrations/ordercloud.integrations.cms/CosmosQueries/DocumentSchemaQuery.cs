using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Schema;
using ordercloud.integrations.cms.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ordercloud.integrations.cms.CosmosQueries
{
	public interface IDocumentSchemaQuery
	{
		Task<ListPage<DocumentSchema>> List(IListArgs args, VerifiedUserContext user);
		Task<DocumentSchema> Get(string schemaInteropID, VerifiedUserContext user);
		Task<DocumentSchema> Create(DocumentSchema schema, VerifiedUserContext user);
		Task<DocumentSchema> Update(string schemaInteropID, DocumentSchema schema, VerifiedUserContext user);
		Task Delete(string schemaInteropID, VerifiedUserContext user);
	}

	public class DocumentSchemaQuery : IDocumentSchemaQuery
	{
		private readonly ICosmosStore<DocumentSchema> _store;

		public DocumentSchemaQuery(ICosmosStore<DocumentSchema> schemaStore)
		{
			_store = schemaStore;
		}

		public async Task<ListPage<DocumentSchema>> List(IListArgs args, VerifiedUserContext user)
		{
			var query = _store.Query(new FeedOptions() { EnableCrossPartitionQuery = true })
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			return list.ToListPage(args.Page, args.PageSize, count);
		}

		public async Task<DocumentSchema> Get(string schemaInteropID, VerifiedUserContext user)
		{
			var schema = await GetWithoutExceptions(schemaInteropID);
			if (schema == null) throw new OrderCloudIntegrationException.NotFoundException("Schema", schemaInteropID);
			return schema;
		}

		public async Task<DocumentSchema> Create(DocumentSchema schema, VerifiedUserContext user)
		{
			var matchingID = await GetWithoutExceptions(schema.InteropID);
			if (matchingID != null) throw new DuplicateIDException();
			Validate(schema);
			var newSchema = await _store.AddAsync(schema);
			return newSchema;
		}

		public async Task<DocumentSchema> Update(string schemaInteropID, DocumentSchema schema, VerifiedUserContext user)
		{
			var existingSchema = await Get(schemaInteropID, user);
			existingSchema.InteropID = schema.InteropID;
			existingSchema.SellerOrgID = schema.SellerOrgID;
			existingSchema.AllowedResourceAssociations = schema.AllowedResourceAssociations;
			existingSchema.Schema = schema.Schema;
			Validate(existingSchema);
			var updatedContainer = await _store.UpdateAsync(existingSchema);
			return updatedContainer;
		}

		public async Task Delete(string schemaInteropID, VerifiedUserContext user)
		{
			var schema = await Get(schemaInteropID, user);
			await _store.RemoveByIdAsync(schema.id, schema.SellerOrgID);
		}

		private void Validate(DocumentSchema schema)
		{
			if (schema.AllowedResourceAssociations.Count < 1)
			{
				throw new AllowedResourceAssociationsEmptyException(schema.InteropID);
			}
			SchemaHelper.ValidateSchema(schema);
		}

		private async Task<DocumentSchema> GetWithoutExceptions(string schemaInteropID)
		{
			var query = $"select top 1 * from c where c.InteropID = @id";
			var schema = await _store.Query(query, new { id = schemaInteropID }).FirstOrDefaultAsync();
			return schema;
		}
	}
}
