using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Mappers;
using Marketplace.CMS.Models;
using Marketplace.CMS.Storage;
using Marketplace.Common;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Marketplace.Helpers.Exceptions.ApiErrorException;

namespace Marketplace.CMS.Queries
{
	public interface IAssetQuery
	{
		Task<ListPage<Asset>> List(string containerInteropID, IListArgs args);
		Task<Asset> Get(string containerInteropID, string assetInteropID);
		Task<Asset> Create(string containerInteropID, AssetUploadForm form);
		//Task<Asset> CreateOrUpdate(string containerInteropID, string assetInteropID, Asset asset);
		Task Delete(string containerInteropID, string assetInteropID);
	}

	public class AssetQuery : IAssetQuery
	{
		private readonly ICosmosStore<Asset> _store;
		private readonly IAssetContainerQuery _containers;
		private readonly IStorageFactory _storageFactory;

		public AssetQuery(ICosmosStore<Asset> store, IAssetContainerQuery containers, IStorageFactory storageFactory)
		{
			_store = store;
			_containers = containers;
			_storageFactory = storageFactory;
		}

		public async Task<ListPage<Asset>> List(string containerInteropID, IListArgs args)
		{
			var container = await _containers.Get(containerInteropID);
			var query = _store.Query(new FeedOptions() { PartitionKey = new PartitionKey(containerInteropID), EnableCrossPartitionQuery = false })
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			return list.ToListPage(args.Page, args.PageSize, count);
		}

		public async Task<Asset> Get(string containerInteropID, string assetInteropID)
		{
			var container = await _containers.Get(containerInteropID);
			var item = await GetWithoutExceptions(containerInteropID, assetInteropID);
			return item ?? throw new NotFoundException("AssetContainer", assetInteropID);
		}

		public async Task<Asset> Create(string containerInteropID, AssetUploadForm form)
		{
			var container = await _containers.Get(containerInteropID);
			var (asset, file) = AssetMapper.Map(container, form);
			if ((await GetWithoutExceptions(containerInteropID, asset.InteropID)) != null) throw new DuplicateIdException();

			return await _store.AddAsync(asset);
		}

		public async Task Delete(string containerInteropID, string assetInteropID)
		{
			await _containers.Get(containerInteropID); // make sure container exists.
			var item = await Get(containerInteropID, assetInteropID);
			await _store.RemoveByIdAsync(item.id, containerInteropID);
		}

		private async Task<Asset> GetWithoutExceptions(string containerInteropID, string assetInteropID)
		{
			var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(containerInteropID) };
			return await _store.Query($"select top 1 * from c where c.InteropID = @id", new { id = assetInteropID }, feedOptions).FirstOrDefaultAsync();
		}
	}
}
