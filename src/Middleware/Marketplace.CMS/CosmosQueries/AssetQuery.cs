using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Mappers;
using Marketplace.CMS.Models;
using Marketplace.CMS.Storage;
using Marketplace.Common;
using Marketplace.Helpers;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Extensions;
using Microsoft.AspNetCore.Http;
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
	public interface IAssetQuery
	{
		Task<ListPage<Asset>> List(string containerInteropID, IListArgs args);
		Task<Asset> Get(string containerInteropID, string assetInteropID);
		Task<Asset> Create(string containerInteropID, AssetUploadForm form);
		Task<Asset> Update(string containerInteropID, string assetInteropID, Asset asset);
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
			var query = _store.Query(new FeedOptions() { PartitionKey = new PartitionKey(container.id), EnableCrossPartitionQuery = false })
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var listPage = list.ToListPage(args.Page, args.PageSize, count);
			listPage.Items = listPage.Items.Select(asset => AssetMapper.MapToResponse(container, asset)).ToList();
			return listPage;
		}

		public async Task<Asset> Get(string containerInteropID, string assetInteropID)
		{
			var container = await _containers.Get(containerInteropID);
			var asset = await GetWithoutExceptions(container.id, assetInteropID);
			if (asset == null) throw new NotFoundException("Asset", assetInteropID);
			return AssetMapper.MapToResponse(container, asset);
		}

		public async Task<Asset> Create(string containerInteropID, AssetUploadForm form)
		{
			var container = await _containers.Get(containerInteropID);
			var (asset, file) = AssetMapper.MapFromUpload(container, form);
			var matchingID = await GetWithoutExceptions(container.id, asset.InteropID);
			if (matchingID != null) throw new DuplicateIdException();
			if (file != null) {			
				await _storageFactory.GetStorage(container).UploadAsset(file, asset);
			}

			var newAsset = await _store.AddAsync(asset);
			return AssetMapper.MapToResponse(container, newAsset);
		}

		public async Task<Asset> Update(string containerInteropID, string assetInteropID, Asset asset)
		{
			var container = await _containers.Get(containerInteropID);
			var existingAsset = await GetWithoutExceptions(container.id, assetInteropID);
			if (existingAsset == null) throw new NotFoundException("Asset", assetInteropID);
			existingAsset.InteropID = asset.InteropID;
			existingAsset.UrlPathOveride = asset.UrlPathOveride;
			existingAsset.Title = asset.Title;
			existingAsset.Active = asset.Active;
			existingAsset.Tags = asset.Tags;
			existingAsset.Type = asset.Type;
			existingAsset.FileName = asset.FileName;
			var updatedAsset = await _store.UpdateAsync(existingAsset);
			return AssetMapper.MapToResponse(container, updatedAsset);
		}


		public async Task Delete(string containerInteropID, string assetInteropID)
		{
			var container = await _containers.Get(containerInteropID); // make sure container exists.
			var asset = await Get(containerInteropID, assetInteropID);
			await _store.RemoveByIdAsync(asset.id, container.id);
			await _storageFactory.GetStorage(container).OnAssetDeleted(asset.id);
		}

		private async Task<Asset> GetWithoutExceptions(string containerID, string assetInteropID)
		{
			var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(containerID) };
			return await _store.Query($"select top 1 * from c where c.InteropID = @id", new { id = assetInteropID }, feedOptions).FirstOrDefaultAsync();
		}
	}
}
