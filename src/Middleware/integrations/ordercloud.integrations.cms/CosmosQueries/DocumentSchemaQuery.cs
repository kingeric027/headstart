using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Schema;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ordercloud.integrations.cms
{
	public interface IDocumentSchemaQuery
	{
		Task<ListPage<DocumentSchemaDO>> List(IListArgs args, VerifiedUserContext user);
		Task<DocumentSchemaDO> Get(string schemaInteropID, VerifiedUserContext user);
		Task<DocumentSchemaDO> GetByInternalID(string schemaID);
		Task<DocumentSchemaDO> Create(DocumentSchemaDO schema, VerifiedUserContext user);
		Task<DocumentSchemaDO> Update(string schemaInteropID, DocumentSchemaDO schema, VerifiedUserContext user);
		Task Delete(string schemaInteropID, VerifiedUserContext user);
	}

	public class DocumentSchemaQuery : IDocumentSchemaQuery
	{
		private readonly ICosmosStore<DocumentSchemaDO> _store;
		private readonly CMSConfig _settings;

		public DocumentSchemaQuery(ICosmosStore<DocumentSchemaDO> schemaStore, CMSConfig settings)
		{
			_store = schemaStore;
			_settings = settings;
		}

		public async Task<ListPage<DocumentSchemaDO>> List(IListArgs args, VerifiedUserContext user)
		{
			var query = _store.Query(GetFeedOptions(user.ClientID))
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			return list.ToListPage(args.Page, args.PageSize, count);
		}

		public async Task<DocumentSchemaDO> GetByInternalID(string schemaID)
		{
			var schema = await _store.Query($"select top 1 * from c where c.id = @id", new { id = schemaID }).FirstOrDefaultAsync();
			return schema;
		}

		public async Task<DocumentSchemaDO> Get(string schemaInteropID, VerifiedUserContext user)
		{
			var schema = await GetWithoutExceptions(schemaInteropID, user);
			if (schema == null) throw new OrderCloudIntegrationException.NotFoundException("Schema", schemaInteropID);
			return schema;
		}

		public async Task<DocumentSchemaDO> Create(DocumentSchemaDO schema, VerifiedUserContext user)
		{
			var matchingID = await GetWithoutExceptions(schema.InteropID, user);
			if (matchingID != null) throw new DuplicateIDException();
			schema = Validate(schema);
			schema.OwnerClientID = user.ClientID;
			schema.History = HistoryBuilder.OnCreate(user);
			var newSchema = await _store.AddAsync(schema);
			return newSchema;
		}

		public async Task<DocumentSchemaDO> Update(string schemaInteropID, DocumentSchemaDO schema, VerifiedUserContext user)
		{
			var existingSchema = await Get(schemaInteropID, user);
			existingSchema.InteropID = schema.InteropID;
			existingSchema.RestrictedAssignmentTypes = schema.RestrictedAssignmentTypes;
			existingSchema.Schema = schema.Schema;
			existingSchema = Validate(existingSchema);
			existingSchema.History = HistoryBuilder.OnUpdate(existingSchema.History, user);

			var updatedSchema = await _store.UpdateAsync(existingSchema);
			return updatedSchema;
		}

		public async Task Delete(string schemaInteropID, VerifiedUserContext user)
		{
			var schema = await Get(schemaInteropID, user);
			await _store.RemoveByIdAsync(schema.id, schema.OwnerClientID);
		}

		private DocumentSchemaDO Validate(DocumentSchemaDO schema)
		{
			schema.Schema["$schema"] = $"{_settings.BaseUrl}/schema-specs/metaschema";
			schema.Schema["$id"] = $"{_settings.BaseUrl}/schema-specs/{schema.id}";
			return SchemaHelper.ValidateSchema(schema);
		}

		private async Task<DocumentSchemaDO> GetWithoutExceptions(string schemaInteropID, VerifiedUserContext user)
		{
			var query = $"select top 1 * from c where c.InteropID = @id";
			var schema = await _store.Query(query, new { id = schemaInteropID }, GetFeedOptions(user.ClientID)).FirstOrDefaultAsync();
			return schema;
		}

		private FeedOptions GetFeedOptions(string apiClientID) => new FeedOptions() { PartitionKey = new PartitionKey(apiClientID) };

	}
}
