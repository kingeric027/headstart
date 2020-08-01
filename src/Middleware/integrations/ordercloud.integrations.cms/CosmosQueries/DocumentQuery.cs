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
		Task<ListPage<Document>> List(string schemaInteropID, IListArgs args, VerifiedUserContext user);
		Task<ListPage<TDoc>> List<TDoc>(string schemaInteropID, IListArgs args, VerifiedUserContext user) where TDoc : Document;
		Task<List<DocumentDO>> ListByInternalIDs(IEnumerable<string> documentIDs);
		Task<Document> Get(string schemaInteropID, string documentInteropID, VerifiedUserContext user);
		Task<DocumentDO> GetDOByInternalID(string documentID); // real id
		Task<DocumentDO> GetDOByInternalSchemaID(string schemaID, string documentInteropID, VerifiedUserContext user);
		Task<Document> Create(string schemaInteropID, Document document, VerifiedUserContext user);
		Task<Document> Update(string schemaInteropID, string documentInteropID, Document document, VerifiedUserContext user);
		Task Delete(string schemaInteropID, string documentInteropID, VerifiedUserContext user);
	}

	public class DocumentQuery: IDocumentQuery
	{
		private readonly ISchemaQuery _schemas;
		private readonly ICosmosStore<DocumentDO> _store;

		public DocumentQuery(ICosmosStore<DocumentDO> schemaStore, ISchemaQuery schemas)
		{
			_store = schemaStore;
			_schemas = schemas;
		}

		public async Task<List<DocumentDO>> ListByInternalIDs(IEnumerable<string> documentIDs)
		{
			return (await _store.FindMultipleAsync(documentIDs)).ToList();
		}

		public async Task<ListPage<TDoc>> List<TDoc>(string schemaInteropID, IListArgs args, VerifiedUserContext user) where TDoc : Document
		{
			var listResponse = await List(schemaInteropID, args, user);
			return listResponse.Reserialize<ListPage<TDoc>>();
		}

		public async Task<ListPage<Document>> List(string schemaInteropID, IListArgs args, VerifiedUserContext user)
		{
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var query = _store.Query(GetFeedOptions(user.ClientID))
				.Search(args)
				.Filter(args)
				.Sort(args)
				.Where(doc => doc.SchemaID == schema.id);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var documents = list.ToListPage(args.Page, args.PageSize, count);
			return DocumentMapper.MapTo(documents);
		}

		public async Task<Document> Get(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			return DocumentMapper.MapTo(await GetDO(schemaInteropID, documentInteropID, user));
		}

		public async Task<DocumentDO> GetDO(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			var schema = await _schemas.GetDO(schemaInteropID, user);
			return await GetDOByInternalSchemaID(schema.id, documentInteropID, user);
		}

		public async Task<DocumentDO> GetDOByInternalID(string documentID) // real id
		{
			var doc = await _store.FindAsync(documentID);
			return doc;
		}

		public async Task<DocumentDO> GetDOByInternalSchemaID(string schemaID, string documentInteropID, VerifiedUserContext user)
		{
			var document = await GetWithoutExceptions(schemaID, documentInteropID, user);
			if (document == null) throw new OrderCloudIntegrationException.NotFoundException("Document", documentInteropID);
			return document;
		}

		public async Task<Document> Create(string schemaInteropID, Document document, VerifiedUserContext user)
		{
			DocumentDO dataObject = DocumentMapper.MapTo(document);
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var matchingID = await GetWithoutExceptions(schema.id, dataObject.InteropID, user);
			if (matchingID != null) throw new DuplicateIDException();
			dataObject = SchemaHelper.ValidateDocumentAgainstSchema(schema, dataObject);
			dataObject.OwnerClientID = user.ClientID;
			dataObject.SchemaID = schema.id;
			dataObject.SchemaSpecUrl = schema.Schema.GetValue("$id").ToString();
			dataObject.History = HistoryBuilder.OnCreate(user);
			var newDocument = await _store.AddAsync(dataObject);
			return DocumentMapper.MapTo(newDocument);
		}

		public async Task<Document> Update(string schemaInteropID, string documentInteropID, Document document, VerifiedUserContext user)
		{
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var existingDocument = await GetDOByInternalSchemaID(schema.id, documentInteropID, user);
			existingDocument.InteropID = document.ID;
			existingDocument.Doc = document.Doc;
			existingDocument = SchemaHelper.ValidateDocumentAgainstSchema(schema, existingDocument);
			existingDocument.History = HistoryBuilder.OnUpdate(existingDocument.History, user);

			var updatedDocument = await _store.UpdateAsync(existingDocument);
			return DocumentMapper.MapTo(updatedDocument);
		}

		public async Task Delete(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var document = await GetDOByInternalSchemaID(schema.id, documentInteropID, user);
			await _store.RemoveByIdAsync(document.id, schema.OwnerClientID);
		}

		private async Task<DocumentDO> GetWithoutExceptions(string schemaID, string documentInteropID, VerifiedUserContext user)
		{
			var document = await _store
				.Query(GetFeedOptions(user.ClientID))
				.FirstOrDefaultAsync(d => d.InteropID == documentInteropID && d.SchemaID == schemaID);
			return document;
		}

		private FeedOptions GetFeedOptions(string apiClientID) => new FeedOptions() { PartitionKey = new PartitionKey(apiClientID) };
	}
}
