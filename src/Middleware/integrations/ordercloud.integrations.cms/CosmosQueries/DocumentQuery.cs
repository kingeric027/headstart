using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
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
		Task<ListPage<Document<TDoc>>> List<TDoc>(string schemaInteropID, IListArgs args, VerifiedUserContext user);
		Task<List<Document<TDoc>>> ListByInternalIDs<TDoc>(IEnumerable<string> documentIDs);
		Task<Document<TDoc>> Get<TDoc>(string schemaInteropID, string documentInteropID, VerifiedUserContext user);
		Task<Document<TDoc>> GetByInternalID<TDoc>(string documentID); // real id
		Task<Document<TDoc>> GetByInternalSchemaID<TDoc>(string schemaID, string documentInteropID, VerifiedUserContext user);
		Task<Document<TDoc>> Create<TDoc>(string schemaInteropID, Document<TDoc> document, VerifiedUserContext user);
		Task<Document<TDoc>> Update<TDoc>(string schemaInteropID, string documentInteropID, Document<TDoc> document, VerifiedUserContext user);
		Task Delete<TDoc>(string schemaInteropID, string documentInteropID, VerifiedUserContext user);
	}

	public class DocumentQuery: IDocumentQuery
	{
		private readonly IDocumentSchemaQuery _schemas;
		private readonly ICosmosStore<Document<JObject>> _store;
		private readonly CMSConfig _settings;

		public DocumentQuery(ICosmosStore<Document<JObject>> schemaStore, CMSConfig settings, IDocumentSchemaQuery schemas)
		{
			_store = schemaStore;
			_settings = settings;
			_schemas = schemas;
		}

		public async Task<List<Document<TDoc>>> ListByInternalIDs<TDoc>(IEnumerable<string> documentIDs)
		{
			var documents = await _store.FindMultipleAsync(documentIDs);
			return documents.Cast<Document<TDoc>>().ToList();
		}

		public async Task<ListPage<Document<TDoc>>> List<TDoc>(string schemaInteropID, IListArgs args, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var query = _store.Query(GetFeedOptions(user.ClientID))
				.Search(args)
				.Filter(args)
				.Sort(args)
				.Where(doc => doc.SchemaID == schema.id);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var listPage = list.ToListPage(args.Page, args.PageSize, count);
			return listPage.Reserialize<ListPage<Document<TDoc>>>();
		}

		public async Task<Document<TDoc>> Get<TDoc>(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			return await GetByInternalSchemaID<TDoc>(schema.id, documentInteropID, user);
		}

		public async Task<Document<TDoc>> GetByInternalID<TDoc>(string documentID) // real id
		{
			var doc = await _store.Query($"select top 1 * from c where c.id = @id", new { id = documentID }).FirstOrDefaultAsync();
			return doc.Reserialize<Document<TDoc>>();
		}

		public async Task<Document<TDoc>> GetByInternalSchemaID<TDoc>(string schemaID, string documentInteropID, VerifiedUserContext user)
		{
			var document = await GetWithoutExceptions<TDoc>(schemaID, documentInteropID, user);
			if (document == null) throw new OrderCloudIntegrationException.NotFoundException("Document", documentInteropID);
			return document;
		}

		public async Task<Document<TDoc>> Create<TDoc>(string schemaInteropID, Document<TDoc> document, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var matchingID = await GetWithoutExceptions<TDoc>(schema.id, document.InteropID, user);
			if (matchingID != null) throw new DuplicateIDException();
			document = SchemaHelper.ValidateDocumentAgainstSchema(schema, document);
			document.OwnerClientID = user.ClientID;
			document.SchemaID = schema.id;
			document.SchemaSpecUrl = schema.Schema.GetValue("$id").ToString();
			var newDocument = await _store.AddAsync(document.Reserialize<Document<JObject>>());
			return newDocument.Entity.Reserialize<Document<TDoc>>();
		}

		public async Task<Document<TDoc>> Update<TDoc>(string schemaInteropID, string documentInteropID, Document<TDoc> document, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var existingDocument = await GetByInternalSchemaID<TDoc>(schema.id, documentInteropID, user);
			existingDocument.InteropID = document.InteropID;
			existingDocument.Doc = document.Doc;
			existingDocument = SchemaHelper.ValidateDocumentAgainstSchema(schema, existingDocument);
			var updatedDocument = await _store.UpdateAsync(existingDocument.Reserialize<Document<JObject>>());
			return updatedDocument.Entity.Reserialize<Document<TDoc>>();
		}

		public async Task Delete<TDoc>(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			var schema = await _schemas.Get(schemaInteropID, user);
			var document = await GetByInternalSchemaID<TDoc>(schema.id, documentInteropID, user);
			await _store.RemoveByIdAsync(document.id, schema.OwnerClientID);
		}

		private async Task<Document<TDoc>> GetWithoutExceptions<TDoc>(string schemaID, string documentInteropID, VerifiedUserContext user)
		{
			var query = $"select top 1 * from c where c.InteropID = @id and c.SchemaID = @schemaID";
			var schema = await _store.Query(query, new { id = documentInteropID, schemaID = schemaID }, GetFeedOptions(user.ClientID)).FirstOrDefaultAsync();
			return schema.Reserialize<Document<TDoc>>();
		}

		private FeedOptions GetFeedOptions(string apiClientID) => new FeedOptions() { PartitionKey = new PartitionKey(apiClientID) };
	}
}
