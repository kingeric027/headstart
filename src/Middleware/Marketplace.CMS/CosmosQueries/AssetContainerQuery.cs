using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Models;
using Marketplace.CMS.Storage;
using Marketplace.Common;
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
		Task<ListPage<AssetContainer>> List(IListArgs args);
		Task<AssetContainer> Get(string interopID);
		Task<AssetContainer> Create(AssetContainer container);
		// Task<AssetContainer> CreateOrUpdate(string interopID, AssetContainer container);
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
			var item = await GetWithoutExceptions(interopID);
			return item ?? throw new NotFoundException("AssetContainer", interopID);
		}

		public async Task<AssetContainer> Create(AssetContainer container)
		{
			if ((await GetWithoutExceptions(container.InteropID)) != null) throw new DuplicateIdException();
			container.StorageAccount = await _storageFactory.GetStorage(container.StorageAccount).OnContainerConnected(container.id);
			container.StorageConnected = true;

			return await _store.AddAsync(container);
		}

		//public async Task<AssetContainer> CreateOrUpdate(string interopID, AssetContainer container)
		//{
		//	var item = await GetWithoutExceptions(interopID);
		//	if (item != null) {
		//		container.id = item.id;
		//		// TODO - how to prevent overriding the readonly fields with null?
		//		return await _store.UpdateAsync(container);
		//	} else
		//	{
		//		return await Create(container);
		//	}
		//}

		public async Task Delete(string interopID)
		{
			var item = await Get(interopID);
			await _storageFactory.GetStorage(item.StorageAccount).OnContainerDeleted(item.id);
			await _store.RemoveByIdAsync(item.id, SinglePartitionID);
			// TODO - delete all the asset records?
		}

		private async Task<AssetContainer> GetWithoutExceptions(string interopID)
		{
			return await _store.Query($"select top 1 * from c where c.InteropID = @id", new { id = interopID }).FirstOrDefaultAsync();
		}
	}
}
