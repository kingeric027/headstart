﻿using Cosmonaut;
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
		Task<ListPage<Document<T>>> List<T>(string schemaInteropID, ListArgs<Document<T>> args, VerifiedUserContext user);
		Task<Document<T>> Get<T>(string schemaInteropID, string documentInteropID, VerifiedUserContext user);
		Task<Document<T>> Create<T>(string schemaInteropID, Document<T> document, VerifiedUserContext user);
		Task<Document<T>> Save<T>(string schemaInteropID, string documentInteropID, Document<T> document, VerifiedUserContext user);
		Task Delete(string schemaInteropID, string documentInteropID, VerifiedUserContext user);

		Task<ListPage<DocumentDO>> ListByInternalIDs(IEnumerable<string> documentIDs, ListArgsPageOnly args = null);
		Task<DocumentDO> GetDOByInternalID(string documentID); // real id
		Task<DocumentDO> GetDOByInternalSchemaID(string schemaID, string documentInteropID, VerifiedUserContext user);
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

		public async Task<ListPage<DocumentDO>> ListByInternalIDs(IEnumerable<string> documentIDs, ListArgsPageOnly args = null)
		{
			args = args ?? new ListArgsPageOnly(1, documentIDs.Count());
			return await _store.FindMultipleAsync(documentIDs, args);
		}

		public async Task<ListPage<Document<T>>> List<T>(string schemaInteropID, ListArgs<Document<T>> args, VerifiedUserContext user)
		{
			var arguments = args.MapTo();
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var query = _store.Query()
				.Search(arguments)
				.Filter(arguments)
				.Sort(arguments)
				.Where(doc => doc.SchemaID == schema.id && doc.SellerOrgID == user.SellerID);
			var list = await query.WithPagination(arguments.Page, arguments.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var documents = list.ToListPage(arguments.Page, arguments.PageSize, count);
			return DocumentMapper.MapTo<T>(documents);
		}

		public async Task<Document<T>> Get<T>(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			return DocumentMapper.MapTo<T>(await GetDO(schemaInteropID, documentInteropID, user));
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
			if (document == null) { throw new OrderCloudIntegrationException.NotFoundException("Document", documentInteropID); }
			return document;
		}

		public async Task<Document<T>> Create<T>(string schemaInteropID, Document<T> document, VerifiedUserContext user)
		{
			DocumentDO dataObject = DocumentMapper.MapTo(document);
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var matchingID = await GetWithoutExceptions(schema.id, dataObject.InteropID, user);
			if (matchingID != null) { throw new DuplicateIDException(); }
			dataObject = Init(dataObject, schema, user);
			dataObject = await SchemaHelper.ValidateDocumentAgainstSchema(schema, dataObject);
			var newDocument = await _store.AddAsync(dataObject);
			return DocumentMapper.MapTo<T>(newDocument);
		}

		public async Task<Document<T>> Save<T>(string schemaInteropID, string documentInteropID, Document<T> document, VerifiedUserContext user)
		{
			var schema = await _schemas.GetDO(schemaInteropID, user);
			if (documentInteropID != document.ID)
			{
				var matchingID = await GetWithoutExceptions(schema.id, document.ID, user);
				if (matchingID != null) { throw new DuplicateIDException(); }
			}
			var existingDocument = await GetWithoutExceptions(schema.id, documentInteropID, user);
			if (existingDocument == null) existingDocument = Init(new DocumentDO(), schema, user);
			existingDocument.InteropID = document.ID;
			existingDocument.Doc = JObject.FromObject(document.Doc);
			existingDocument.History = HistoryBuilder.OnUpdate(existingDocument.History, user);
			existingDocument = await SchemaHelper.ValidateDocumentAgainstSchema(schema, existingDocument);
			var updatedDocument = await _store.UpsertAsync(existingDocument);
			return DocumentMapper.MapTo<T>(updatedDocument);
		}

		public async Task Delete(string schemaInteropID, string documentInteropID, VerifiedUserContext user)
		{
			var schema = await _schemas.GetDO(schemaInteropID, user);
			var document = await GetDOByInternalSchemaID(schema.id, documentInteropID, user);
			await _store.RemoveByIdAsync(document.id, schema.SellerOrgID);
		}

		private async Task<DocumentDO> GetWithoutExceptions(string schemaID, string documentInteropID, VerifiedUserContext user)
		{
			var document = await _store
				.Query()
				.FirstOrDefaultAsync(d => d.InteropID == documentInteropID && d.SchemaID == schemaID && d.SellerOrgID == user.SellerID);
			return document;
		}

		private DocumentDO Init(DocumentDO doc, DocSchemaDO schema, VerifiedUserContext user)
		{
			doc.SellerOrgID = user.SellerID;
			doc.SchemaID = schema.id;
			doc.SchemaSpecUrl = schema.Schema.GetValue("$id").ToString();
			doc.History = HistoryBuilder.OnCreate(user);
			return doc;
		}
	}
}
