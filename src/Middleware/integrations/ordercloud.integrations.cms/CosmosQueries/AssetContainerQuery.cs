using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	public interface IAssetContainerQuery
	{
		Task<AssetContainer> CreateDefaultIfNotExists(VerifiedUserContext user);
		Task<ListPage<AssetContainer>> List(IListArgs args);
		Task<AssetContainer> Get(string interopID);
		Task<AssetContainer> Create(AssetContainer container);
		Task<AssetContainer> Update(string interopID, AssetContainer container);
		Task Delete(string interopID);
	}

	public class AssetContainerQuery: IAssetContainerQuery
	{
		private readonly ICosmosStore<AssetContainer> _store;
		private readonly IBlobStorage _blob;
		public const string SinglePartitionID = "SinglePartitionID"; // TODO - is there a better way?

		public AssetContainerQuery(ICosmosStore<AssetContainer> store, IBlobStorage blob)
		{
			_store = store;
			_blob = blob;
		}

		public async Task<ListPage<AssetContainer>> List(IListArgs args)
		{
			var query = _store.Query(new FeedOptions() { EnableCrossPartitionQuery = false })
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			return list.ToListPage(args.Page, args.PageSize, count);
		}

		public async Task<AssetContainer> Get(string interopID)
		{
			var container = await GetWithoutExceptions(interopID);
			if (container == null) throw new OrderCloudIntegrationException.NotFoundException("AssetContainer", interopID);
			return container;
		}

		public async Task<AssetContainer> CreateDefaultIfNotExists(VerifiedUserContext user)
		{
			var defaultContainer = new AssetContainer()
			{
				InteropID = user.ClientID,
				Name = $"Container for API Client with ID {user.ClientID}"
			};
			var existingContainer = await GetWithoutExceptions(defaultContainer.InteropID);
			return existingContainer ?? await Create(defaultContainer);
		}

		public async Task<AssetContainer> Create(AssetContainer container)
		{
			var matchingID = await GetWithoutExceptions(container.InteropID);
			if (matchingID != null) throw new DuplicateIdException("IdExists", "Object already exists.", null);

			var newContainer = await _store.AddAsync(container);
			return newContainer;
		}

		public async Task<AssetContainer> Update(string interopID, AssetContainer container)
		{
			var existingContainer = await Get(interopID);
			existingContainer.InteropID = container.InteropID;
			existingContainer.Name = container.Name;
			var updatedContainer = await _store.UpdateAsync(existingContainer);
			return updatedContainer;
		}

		public async Task Delete(string interopID)
		{
			var container = await Get(interopID);
			await _blob.OnContainerDeleted(container);
			await _store.RemoveByIdAsync(container.id, SinglePartitionID);
			// TODO - delete all the asset records in cosmos?
		}

		private async Task<AssetContainer> GetWithoutExceptions(string interopID)
		{
			return await _store.Query($"select top 1 * from c where c.InteropID = @id", new { id = interopID }).FirstOrDefaultAsync();
		}
	}
}
