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
		Task<List<Asset>> ListAcrossContainers(IEnumerable<string> assetIDs);
		Task<Asset> GetAcrossContainers(string assetID);
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
		private readonly IBlobStorage _blob;

		public AssetQuery(ICosmosStore<Asset> assetStore, IAssetContainerQuery containers, IBlobStorage blob)
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
			var listPage = list.ToListPage(args.Page, args.PageSize, count);
			return listPage;
		}

		public async Task<Asset> Get(string assetInteropID, VerifiedUserContext user)
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
			if (matchingID != null) throw new DuplicateIdException("IdExists", "Object already exists.", null);
			if (file != null) {			
				await _blob.UploadAsset(container, file, asset);
			}

			var newAsset = await _assetStore.AddAsync(asset);
			return newAsset;
		}

		public async Task<Asset> Update(string assetInteropID, Asset asset, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var existingAsset = await GetWithoutExceptions(container.id, assetInteropID);
			if (existingAsset == null) throw new OrderCloudIntegrationException.NotFoundException("Asset", assetInteropID);
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
			return updatedAsset;
		}

		public async Task Delete(string assetInteropID, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = await Get(assetInteropID, user);
			await _assetStore.RemoveByIdAsync(asset.id, container.id);
			await _blob.OnAssetDeleted(container, asset.id);
		}

		public async Task<List<Asset>> ListAcrossContainers(IEnumerable<string> assetIDs)
		{ 
			var ids = new HashSet<string>(assetIDs).ToList(); // remove any duplicates
			if (ids.Count == 0) return new List<Asset>(); 
			var paramNames = ids.Select((id, i) => $"@id{i}");
			var parameters = new ExpandoObject();
			for (int i = 0; i < ids.Count; i++)
			{
				parameters.TryAdd($"@id{i}", ids[i]);
			}
			var assets = await _assetStore.QueryMultipleAsync($"select * from c where c.id IN ({string.Join(", ", paramNames)})", parameters);
			return assets.ToList();
		}

		public async Task<Asset> GetAcrossContainers(string assetID)
		{
			var asset = await _assetStore.Query($"select top 1 * from c where c.id = @id", new { id = assetID }).FirstOrDefaultAsync();
			if (asset == null) throw new NotImplementedException();
			return asset;
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
