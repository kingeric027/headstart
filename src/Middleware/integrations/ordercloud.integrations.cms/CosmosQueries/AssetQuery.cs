using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace ordercloud.integrations.cms
{
	public interface IAssetQuery
	{
		Task<ListPage<Asset>> List(IListArgs args, VerifiedUserContext user);
		Task<Asset> Get(string assetInteropID, VerifiedUserContext user);
		Task<Asset> Create(AssetUpload form, VerifiedUserContext user);
		Task<Asset> Save(string assetInteropID, Asset asset, VerifiedUserContext user);
		Task Delete(string assetInteropID, VerifiedUserContext user);

		Task<List<AssetDO>> ListByInternalIDs(IEnumerable<string> assetIDs);
		Task<AssetDO> GetDO(string assetInteropID, VerifiedUserContext user);
		Task<AssetDO> GetByInternalID(string assetID); // real id
	}

	public class AssetQuery : IAssetQuery
	{
		private readonly ICosmosStore<AssetDO> _assetStore;
		private readonly IAssetContainerQuery _containers;
		private readonly IBlobStorage _blob;

		public AssetQuery(ICosmosStore<AssetDO> assetStore, IAssetContainerQuery containers, IBlobStorage blob)
		{
			_assetStore = assetStore;
			_containers = containers;
			_blob = blob;
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
			var assets = list.ToListPage(args.Page, args.PageSize, count);
			return AssetMapper.MapTo(assets);
		}

		public async Task<Asset> Get(string assetInteropID, VerifiedUserContext user)
		{
			return AssetMapper.MapTo(await GetDO(assetInteropID, user));
		}

		public async Task<AssetDO> GetDO(string assetInteropID, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = await GetWithoutExceptions(container.id, assetInteropID);
			if (asset == null) throw new OrderCloudIntegrationException.NotFoundException("Asset", assetInteropID);
			return asset;
		}

		public async Task<Asset> Create(AssetUpload form, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var (asset, file) = AssetMapper.MapFromUpload(_blob.Config, container, form);
			var matchingID = await GetWithoutExceptions(container.id, asset.InteropID);
			if (matchingID != null) throw new DuplicateIDException();
			if (file != null) {			
				await _blob.UploadAsset(container, file, asset);
			}
			asset.History = HistoryBuilder.OnCreate(user);
			var newAsset = await _assetStore.AddAsync(asset);
			return AssetMapper.MapTo(newAsset);
		}

		public async Task<Asset> Save(string assetInteropID, Asset asset, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var existingAsset = await GetWithoutExceptions(container.id, assetInteropID);
			if (existingAsset == null) {
				if (asset.Url == null) throw new AssetCreateValidationException("Must include a Url");
				existingAsset = new AssetDO()
				{
					ContainerID = container.id,
					History = HistoryBuilder.OnCreate(user),
					Metadata = new AssetMetadata() { IsUrlOverridden = true  }
				};
			}
			existingAsset.InteropID = asset.ID;
			existingAsset.Title = asset.Title;
			existingAsset.Active = asset.Active;
			if (existingAsset.Metadata.IsUrlOverridden)
			{
				existingAsset.Url = asset.Url; // Don't allow changing the url if its generated.
			}
			existingAsset.Tags = asset.Tags;
			existingAsset.FileName = asset.FileName;
			existingAsset.History = HistoryBuilder.OnUpdate(existingAsset.History, user);

			// Intentionally don't allow changing the type. Could mess with assignments.
			var updatedAsset = await _assetStore.UpdateAsync(existingAsset);
			return AssetMapper.MapTo(updatedAsset);
		}

		public async Task Delete(string assetInteropID, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = await GetWithoutExceptions(container.id, assetInteropID);
			await _assetStore.RemoveByIdAsync(asset.id, container.id);
			await _blob.OnAssetDeleted(container, asset.id);
		}

		public async Task<List<AssetDO>> ListByInternalIDs(IEnumerable<string> assetIDs)
		{
			var assets = await _assetStore.FindMultipleAsync(assetIDs);
			return assets.ToList();
		}

		public async Task<AssetDO> GetByInternalID(string assetID)
		{
			var asset = await _assetStore.Query().FirstOrDefaultAsync(a => a.id == assetID);
			if (asset == null) throw new NotImplementedException(); // Why not implemented instead of not found?
			return asset;
		}

		private async Task<AssetDO> GetWithoutExceptions(string containerID, string assetInteropID)
		{
			var asset = await _assetStore.Query(GetFeedOptions(containerID)).FirstOrDefaultAsync(a => a.InteropID == assetInteropID);
			return asset;
		}

		private FeedOptions GetFeedOptions(string containerID) => new FeedOptions() { PartitionKey = new PartitionKey(containerID) };
 	}
}
