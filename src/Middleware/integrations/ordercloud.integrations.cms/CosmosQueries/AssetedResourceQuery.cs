using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	public interface IAssetedResourceQuery
	{
		Task<List<AssetForDelivery>> ListAssets(Resource resource, VerifiedUserContext user);
		Task<string> GetFirstImage(Resource resource, VerifiedUserContext user);
		Task SaveAssignment(Resource resource, string assetID, VerifiedUserContext user);
		Task DeleteAssignment(Resource resource, string assetID, VerifiedUserContext user);
		Task MoveAssignment(Resource resource, string assetID, int listOrderWithinType, VerifiedUserContext user);
	}

	public class AssetedResourceQuery : IAssetedResourceQuery
	{
		private readonly ICosmosStore<AssetedResource> _store;
		private readonly IAssetQuery _assets;
		private readonly IBlobStorage _blob;

		public AssetedResourceQuery(ICosmosStore<AssetedResource> store, IAssetQuery assets, IBlobStorage blob)
		{
			_store = store;
			_assets = assets;
			_blob = blob;
		}

		public async Task<List<AssetForDelivery>> ListAssets(Resource resource, VerifiedUserContext user)
		{
			// Confirm user has access to resource.
			// await new MultiTenantOCClient(user).Get(resource); Commented out until I solve visiblity for /me endpoints
			var assetedResource = await GetExisting(resource);
			if (assetedResource == null) return new List<AssetForDelivery>();
			var assetIDs = assetedResource.ImageAssetIDs
				.Concat(assetedResource.ThemeAssetIDs)
				.Concat(assetedResource.AttachmentAssetIDs)
				.Concat(assetedResource.StructuredAssetsIDs)
				.ToList();
			var assets = await _assets.ListAcrossContainers(assetIDs);
			return assets.Select(asset => {
				int indexWithinType = GetAssetIDs(assetedResource, asset.Type).IndexOf(asset.id);
				return new AssetForDelivery(asset, indexWithinType);
			}).OrderBy(c => c.Type).ThenBy(c => c.ListOrderWithinType).ToList();
		}

		public async Task<string> GetFirstImage(Resource resource, VerifiedUserContext user)
		{
			var assetedResource = await GetExisting(resource);
			if (assetedResource?.ImageAssetIDs == null || assetedResource.ImageAssetIDs.Count == 0)
			{
				return GetPlaceholderImageUrl(resource.Type);
			}
			var asset = await _assets.GetAcrossContainers(assetedResource.ImageAssetIDs.First());
			return asset.Url;
		}

		public async Task SaveAssignment(Resource resource, string assetID, VerifiedUserContext user)
		{
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var asset = await _assets.Get(assetID, user);
			var assetedResource = await GetExistingOrDefault(resource);
			GetAssetIDs(assetedResource, asset.Type).UniqueAdd(asset.id); 
			await _store.UpsertAsync(assetedResource);
		}

		public async Task DeleteAssignment(Resource resource, string assetID, VerifiedUserContext user)
		{
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var asset = await _assets.Get(assetID, user);
			var assetedResource = await GetExistingOrDefault(resource);
			GetAssetIDs(assetedResource, asset.Type).Remove(asset.id);
			await _store.UpdateAsync(assetedResource);
		}

		public async Task MoveAssignment(Resource resource, string assetID, int listOrderWithinType, VerifiedUserContext user)
		{
			await new OrderCloudClientWithContext(user).EmptyPatch(resource);
			var asset = await _assets.Get(assetID, user);
			var assetedResource = await GetExistingOrDefault(resource);
			GetAssetIDs(assetedResource, asset.Type).MoveTo(asset.id, listOrderWithinType);
			await _store.UpdateAsync(assetedResource);
		}

		private async Task<AssetedResource> GetExistingOrDefault(Resource resource)
		{
			return await GetExisting(resource) ?? new AssetedResource() { Resource = resource };
		}

		private async Task<AssetedResource> GetExisting(Resource resource)
		{
			var query = $"select top 1 * from c where c.Resource.Type = @Type AND c.Resource.ID = @ID AND c.Resource.ParentID = @ParentID";
			var assetedResource = await _store.QuerySingleAsync(query, resource);
			return assetedResource;
		}

		private List<string> GetAssetIDs(AssetedResource assetedResource, AssetType assetType)
		{
			var property = assetedResource.GetType().GetProperty($"{assetType.ToString()}AssetIDs");
			var list = (List<string>)property.GetValue(assetedResource, null) ?? new List<string>();
			return list;
		}

		private string GetPlaceholderImageUrl(ResourceType type)
		{
			var url = $"{_blob.Config.BlobStorageHostUrl}/placeholder-images";
			switch (type)
			{
				case ResourceType.Products:
					return $"{url}/product";
				default:
					return $"{url}/generic";
			}
		}
	}
}