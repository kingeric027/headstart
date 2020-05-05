using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Mappers;
using Marketplace.CMS.Models;
using Marketplace.CMS.Storage;
using Marketplace.Helpers;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Marketplace.Helpers.Exceptions.ApiErrorException;

namespace Marketplace.CMS.Queries
{
	public interface IAssetQuery
	{
		Task<IDictionary<string, Asset>> List(AssetContainer container, ISet<string> assetIDs);
		Task<ListPage<Asset>> List(IListArgs args, VerifiedUserContext user);
		Task<Asset> Get(string assetInteropID, VerifiedUserContext user);
		Task<Asset> Create(AssetUpload form, VerifiedUserContext user);
		Task<Asset> Update(string assetInteropID, Asset asset, VerifiedUserContext user);
		Task Delete(string assetInteropID, VerifiedUserContext user);
	}

	public class AssetQuery : IAssetQuery
	{
		private readonly ICosmosStore<Asset> _assetStore;
		private readonly IAssetContainerQuery _containers;
		private readonly IStorageFactory _storageFactory;

		public AssetQuery(ICosmosStore<Asset> assetStore, IAssetContainerQuery containers, IStorageFactory storageFactory)
		{
			_assetStore = assetStore;
			_containers = containers;
			_storageFactory = storageFactory;
		}

		public async Task<ListPage<Asset>> List(IListArgs args, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var query = _assetStore.Query(GetFeedOptions(container.id))
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var listPage = list.ToListPage(args.Page, args.PageSize, count);
			listPage.Items = listPage.Items.Select(asset => AssetMapper.MapToResponse(container, asset)).ToList();
			return listPage;
		}

		public async Task<Asset> Get(string assetInteropID, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = await GetWithoutExceptions(container.id, assetInteropID);
			if (asset == null) throw new NotFoundException("Asset", assetInteropID);
			return AssetMapper.MapToResponse(container, asset);
		}

		public async Task<Asset> Create(AssetUpload form, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var (asset, file) = AssetMapper.MapFromUpload(container, form);
			var matchingID = await GetWithoutExceptions(container.id, asset.InteropID);
			if (matchingID != null) throw new DuplicateIdException();
			if (file != null) {			
				await _storageFactory.GetStorage(container).UploadAsset(file, asset);
			}

			var newAsset = await _assetStore.AddAsync(asset);
			return AssetMapper.MapToResponse(container, newAsset);
		}

		public async Task<Asset> Update(string assetInteropID, Asset asset, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var existingAsset = await GetWithoutExceptions(container.id, assetInteropID);
			if (existingAsset == null) throw new NotFoundException("Asset", assetInteropID);
			existingAsset.InteropID = asset.InteropID;
			existingAsset.Title = asset.Title;
			existingAsset.Active = asset.Active;
			if (existingAsset.Metadata.IsUrlOverridden)
			{
				existingAsset.Url = asset.Url; // Don't allow changing the url if its generated.
			}
			existingAsset.Tags = asset.Tags;
			existingAsset.FileName = asset.FileName;
			// Intentionally don't allow changing the type. Could mess with assignments.
			var updatedAsset = await _assetStore.UpdateAsync(existingAsset);
			return AssetMapper.MapToResponse(container, updatedAsset);
		}

		public async Task Delete(string assetInteropID, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = await Get(assetInteropID, user);
			await _assetStore.RemoveByIdAsync(asset.id, container.id);
			await _storageFactory.GetStorage(container).OnAssetDeleted(asset.id);
		}

		public async Task<IDictionary<string, Asset>> List(AssetContainer container, ISet<string> assetIDs)
		{
			var ids = assetIDs.ToList();
			var paramNames = ids.Select((id, i) => $"@id{i}");
			var parameters = new ExpandoObject();
			for (int i = 0; i < ids.Count; i++)
			{
				parameters.TryAdd($"@id{i}", ids[i]);
			}
			var query = $"select * from c where c.id IN ({string.Join(", ", paramNames)})";
			var assets = await _assetStore.QueryMultipleAsync(query, parameters, GetFeedOptions(container.id));
			return assets.Select(asset => AssetMapper.MapToResponse(container, asset)).ToDictionary(x => x.id);
		}

		private async Task<Asset> GetWithoutExceptions(string containerID, string assetInteropID)
		{
			var query = $"select top 1 * from c where c.InteropID = @id";
			var asset = await _assetStore.Query(query, new { id = assetInteropID }, GetFeedOptions(containerID)).FirstOrDefaultAsync();
			return asset;
		}

		private FeedOptions GetFeedOptions(string containerID) => new FeedOptions() { PartitionKey = new PartitionKey(containerID) };
 	}
}
