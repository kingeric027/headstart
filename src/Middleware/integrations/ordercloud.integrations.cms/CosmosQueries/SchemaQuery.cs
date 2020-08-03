using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace ordercloud.integrations.cms
{
	public interface ISchemaQuery
	{
		Task<ListPage<DocSchema>> List(IListArgs args, VerifiedUserContext user);
		Task<DocSchema> Get(string schemaInteropID, VerifiedUserContext user);
		Task<DocSchema> GetByInternalID(string schemaID);
		Task<DocSchemaDO> GetDO(string schemaInteropID, VerifiedUserContext user);
		Task<DocSchema> Create(DocSchema schema, VerifiedUserContext user);
		Task<DocSchema> Update(string schemaInteropID, DocSchema schema, VerifiedUserContext user);
		Task Delete(string schemaInteropID, VerifiedUserContext user);
	}

	public class SchemaQuery : ISchemaQuery
	{
		private readonly ICosmosStore<DocSchemaDO> _store;
		private readonly CMSConfig _settings;

		public SchemaQuery(ICosmosStore<DocSchemaDO> schemaStore, CMSConfig settings)
		{
			_store = schemaStore;
			_settings = settings;
		}

		public async Task<ListPage<DocSchema>> List(IListArgs args, VerifiedUserContext user)
		{
			var query = _store.Query(GetFeedOptions(user.SellerID))
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var schemas = list.ToListPage(args.Page, args.PageSize, count);
			return SchemaMapper.MapTo(schemas);
		}

		public async Task<DocSchema> GetByInternalID(string schemaID)
		{
			var schema = await _store.FindAsync(schemaID);
			return SchemaMapper.MapTo(schema);
		}

		public async Task<DocSchema> Get(string schemaInteropID, VerifiedUserContext user)
		{
			var mappedSchema = SchemaMapper.MapTo(await GetDO(schemaInteropID, user));
			return mappedSchema;
		}

		public async Task<DocSchemaDO> GetDO(string schemaInteropID, VerifiedUserContext user)
		{
			var schema = await GetWithoutExceptions(schemaInteropID, user);
			if (schema == null) throw new OrderCloudIntegrationException.NotFoundException("Schema", schemaInteropID);
			return schema;
		}

		public async Task<DocSchema> Create(DocSchema schema, VerifiedUserContext user)
		{
			DocSchemaDO dataObject = SchemaMapper.MapTo(schema);
			var matchingID = await GetWithoutExceptions(dataObject.InteropID, user);
			if (matchingID != null) throw new DuplicateIDException();
			dataObject.SellerOrgID = user.SellerID;
			dataObject.History = HistoryBuilder.OnCreate(user);
			dataObject = Validate(dataObject);
			var newSchema = await _store.AddAsync(dataObject);
			return SchemaMapper.MapTo(newSchema);
		}

		public async Task<DocSchema> Update(string schemaInteropID, DocSchema schema, VerifiedUserContext user)
		{
			var existingSchema = await GetDO(schemaInteropID, user);
			existingSchema.InteropID = schema.ID;
			existingSchema.RestrictedAssignmentTypes = schema.RestrictedAssignmentTypes;
			existingSchema.Schema = schema.Schema;
			existingSchema = Validate(existingSchema);
			existingSchema.History = HistoryBuilder.OnUpdate(existingSchema.History, user);
			var updatedSchema = await _store.UpdateAsync(existingSchema);
			return SchemaMapper.MapTo(updatedSchema);
		}

		public async Task Delete(string schemaInteropID, VerifiedUserContext user)
		{
			var schema = await GetDO(schemaInteropID, user);
			await _store.RemoveByIdAsync(schema.id, schema.SellerOrgID);
		}

		private DocSchemaDO Validate(DocSchemaDO schema)
		{
			schema.Schema["$schema"] = $"{_settings.BaseUrl}/schema-specs/metaschema";
			schema.Schema["$id"] = $"{_settings.BaseUrl}/schema-specs/{schema.id}";
			return SchemaHelper.ValidateSchema(schema);
		}

		private async Task<DocSchemaDO> GetWithoutExceptions(string schemaInteropID, VerifiedUserContext user)
		{
			var schema = await _store.Query(GetFeedOptions(user.SellerID)).FirstOrDefaultAsync(s => s.InteropID == schemaInteropID);
			return schema;
		}

		private FeedOptions GetFeedOptions(string sellerOrgID) => new FeedOptions() { PartitionKey = new PartitionKey(sellerOrgID) };

	}
}
