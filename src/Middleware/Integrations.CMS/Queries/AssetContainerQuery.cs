using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Models;
using Marketplace.CMS.Storage;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.Services;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Marketplace.Helpers.Exceptions.ApiErrorException;

namespace Marketplace.CMS.Queries
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
		private readonly IStorageFactory _storageFactory;
		public const string SinglePartitionID = "SinglePartitionID"; // TODO - is there a better way?

		public AssetContainerQuery(ICosmosStore<AssetContainer> store, IStorageFactory storageFactory)
		{
			_store = store;
			_storageFactory = storageFactory;
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
			if (container == null) throw new NotFoundException("AssetContainer", interopID);
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
			if (matchingID != null) throw new DuplicateIdException();
			container = await _storageFactory.GetStorage(container).OnContainerConnected();
			container.StorageAccount.Connected = true;

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
			await _storageFactory.GetStorage(container).OnContainerDeleted();
			await _store.RemoveByIdAsync(container.id, SinglePartitionID);
			// TODO - delete all the asset records in cosmos?
		}

		private async Task<AssetContainer> GetWithoutExceptions(string interopID)
		{
			return await _store.Query($"select top 1 * from c where c.InteropID = @id", new { id = interopID }).FirstOrDefaultAsync();
		}
	}
}
