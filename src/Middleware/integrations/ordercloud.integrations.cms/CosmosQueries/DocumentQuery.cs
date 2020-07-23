using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Schema;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ordercloud.integrations.cms
{
	public interface IDocumentQuery
	{
		Task<ListPage<DocumentDO>> List(string schemaInteropID, IListArgs args, VerifiedUserContext user);
		Task<List<DocumentDO>> ListByInternalIDs(IEnumerable<string> documentIDs);
		Task<DocumentDO> Get(string schemaInteropID, string documentInteropID, VerifiedUserContext user);
		Task<DocumentDO> GetByInternalID(string documentID); // real id
		Task<DocumentDO> GetByInternalSchemaID(string schemaID, string documentInteropID, VerifiedUserContext user);
		Task<DocumentDO> Create(string schemaInteropID, DocumentDO document, VerifiedUserContext user);
		Task<DocumentDO> Update(string schemaInteropID, string documentInteropID, DocumentDO document, VerifiedUserContext user);
		Task Delete(string schemaInteropID, string documentInteropID, VerifiedUserContext user);
	}

	public class DocumentQuery: IDocumentQuery
	{
		private readonly IDocumentSchemaQuery _schemas;
		private readonly ICosmosStore<DocumentDO> _store;
		private readonly CMSConfig _settings;

		public DocumentQuery(ICosmosStore<DocumentDO> schemaStore, CMSConfig settings, IDocumentSchemaQuery schemas)
		{
			_store = schemaStore;
			_settings = settings;
			_schemas = schemas;
		}

		public async Task<List<DocumentDO>> ListByInternalIDs(IEnumerable<string> documentIDs)
		{
			var documents = await _store.FindMultipleAsync(documentIDs);
			return documents.ToList();
		}

		public async Task<ListPage<DocumentDO>> List(string schemaInteropID, IListArgs args, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var query = _store.Query(GetFeedOptions(user.ClientID))
				.Search(args)
				.Filter(args)
				.Sort(args)
				.Where(doc => doc.SchemaID == schema.id);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			return list.ToListPage(args.Page, args.PageSize, count);
		}

		public async Task<DocumentDO> Get(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			return await GetByInternalSchemaID(schema.id, documentInteropID, user);
		}

		public async Task<DocumentDO> GetByInternalID(string documentID) // real id
		{
			var doc = await _store.Query($"select top 1 * from c where c.id = @id", new { id = documentID }).FirstOrDefaultAsync();
			return doc;
		}

		public async Task<DocumentDO> GetByInternalSchemaID(string schemaID, string documentInteropID, VerifiedUserContext user)
		{
			var document = await GetWithoutExceptions(schemaID, documentInteropID, user);
			if (document == null) throw new OrderCloudIntegrationException.NotFoundException("Document", documentInteropID);
			return document;
		}

		public async Task<DocumentDO> Create(string schemaInteropID, DocumentDO document, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var matchingID = await GetWithoutExceptions(schema.id, document.InteropID, user);
			if (matchingID != null) throw new DuplicateIDException();
			document = SchemaHelper.ValidateDocumentAgainstSchema(schema, document);
			document.OwnerClientID = user.ClientID;
			document.SchemaID = schema.id;
			document.SchemaSpecUrl = schema.Schema.GetValue("$id").ToString();
			document.History = HistoryBuilder.OnCreate(user);
			var newDocument = await _store.AddAsync(document);
			return newDocument;
		}

		public async Task<DocumentDO> Update(string schemaInteropID, string documentInteropID, DocumentDO document, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var existingDocument = await GetByInternalSchemaID(schema.id, documentInteropID, user);
			existingDocument.InteropID = document.InteropID;
			existingDocument.Doc = document.Doc;
			existingDocument = SchemaHelper.ValidateDocumentAgainstSchema(schema, existingDocument);
			existingDocument.History = HistoryBuilder.OnUpdate(existingDocument.History, user);

			var updatedDocument = await _store.UpdateAsync(existingDocument);
			return updatedDocument;
		}

		public async Task Delete(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var document = await GetByInternalSchemaID(schema.id, documentInteropID, user);
			await _store.RemoveByIdAsync(document.id, schema.OwnerClientID);
		}

		private async Task<DocumentDO> GetWithoutExceptions(string schemaID, string documentInteropID, VerifiedUserContext user)
		{
			var query = $"select top 1 * from c where c.InteropID = @id and c.SchemaID = @schemaID";
			var schema = await _store.Query(query, new { id = documentInteropID, schemaID = schemaID }, GetFeedOptions(user.ClientID)).FirstOrDefaultAsync();
			return schema;
		}

		private FeedOptions GetFeedOptions(string apiClientID) => new FeedOptions() { PartitionKey = new PartitionKey(apiClientID) };
	}
}
