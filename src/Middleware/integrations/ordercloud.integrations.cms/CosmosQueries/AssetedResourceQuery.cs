﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace ordercloud.integrations.cms
{
	public interface IAssetedResourceQuery
	{
		Task<ListPage<Asset>> ListAssets(Resource resource, ListArgsPageOnly args, VerifiedUserContext user);
		Task<string> GetThumbnail(Resource resource, ThumbSize size, string sellerID);
		Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task MoveImageAssignment(AssetAssignment assignment, int newPosition, VerifiedUserContext user);
	}

	public class AssetedResourceQuery : IAssetedResourceQuery
	{
		private readonly ICosmosStore<AssetedResourceDO> _store;
		private readonly IAssetQuery _assets;
		private readonly CMSConfig _config;

		public AssetedResourceQuery(ICosmosStore<AssetedResourceDO> store, IAssetQuery assets, CMSConfig config)
		{
			_store = store;
			_assets = assets;
			_config = config;
		}

		public async Task<ListPage<Asset>> ListAssets(Resource resource, ListArgsPageOnly args, VerifiedUserContext user)
		{
			resource.Validate();
			args = args ?? new ListArgsPageOnly();
			// Confirm user has access to resource.
			// await new MultiTenantOCClient(user).Get(resource); Commented out until I solve visiblity for /me endpoints
			var assetedResource = await GetExisting(resource, user.SellerID);
			if (assetedResource == null) return new ListPage<Asset>().Empty();
			var assetIDs = assetedResource.ImageAssetIDs
				.Concat(assetedResource.AllOtherAssetIDs)
				.ToList();
			var assets = await _assets.ListByInternalIDs(assetIDs, args);
			var items = assets.Items.Select(a => {
				int indexWithinType = GetAssetIDs(assetedResource, a.Type).IndexOf(a.id);
				var asset = AssetMapper.MapTo(_config, a);
				return (asset, indexWithinType);
			}).OrderBy(c => c.asset.Type).ThenBy(c => c.indexWithinType).Select(x => x.asset).ToList();
			return new ListPage<Asset>()
			{
				Items = items,
				Meta = assets.Meta
			};
		}

		public async Task<string> GetThumbnail(Resource resource, ThumbSize size, string sellerID)
		{
			resource.Validate();
			var assetedResource = await GetExisting(resource, sellerID);
			if (assetedResource?.ImageAssetIDs == null || assetedResource.ImageAssetIDs.Count == 0)
			{
				return GetPlaceholderImageUrl(resource.ResourceType ?? 0);
			}
			var asset = await _assets.GetByInternalID(assetedResource.ImageAssetIDs.First());
			return asset.Url ?? $"{_config.BlobStorageHostUrl}/assets-{asset.ContainerID}/{asset.id}-{size.ToString().ToLower()}";
		}

		public async Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			assignment.Validate();
			await new OrderCloudClientWithContext(user).EmptyPatch(assignment);
			var asset = await _assets.GetDO(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment, user.SellerID);
			GetAssetIDs(assetedResource, asset.Type).UniqueAdd(asset.id); 
			await _store.UpsertAsync(assetedResource);
		}

		public async Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			assignment.Validate();
			await new OrderCloudClientWithContext(user).EmptyPatch(assignment);
			var asset = await _assets.GetDO(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment, user.SellerID);
			GetAssetIDs(assetedResource, asset.Type).Remove(asset.id);
			await _store.UpdateAsync(assetedResource);
		}

		public async Task MoveImageAssignment(AssetAssignment assignment, int newPosition, VerifiedUserContext user)
		{
			assignment.Validate();
			await new OrderCloudClientWithContext(user).EmptyPatch(assignment);
			var asset = await _assets.GetDO(assignment.AssetID, user);
			if (asset.Type != AssetType.Image) throw new ReorderImagesOnlyException();
			var assetedResource = await GetExistingOrDefault(assignment, user.SellerID);
			assetedResource.ImageAssetIDs.MoveTo(asset.id, newPosition);
			await _store.UpdateAsync(assetedResource);
		}

		private async Task<AssetedResourceDO> GetExistingOrDefault(Resource resource, string sellerID)
		{
			return await GetExisting(resource, sellerID) ?? new AssetedResourceDO() { 
				SellerOrgID = sellerID,
				ResourceID = resource.ResourceID, 
				ResourceParentID = resource.ParentResourceID, 
				ResourceType = resource.ResourceType ?? 0 // "Required" validation should prevent null ResourceType
			};
		}

		private async Task<AssetedResourceDO> GetExisting(Resource resource, string sellerID)
		{
			var assetedResource = await _store.Query().FirstOrDefaultAsync(a => 
				a.SellerOrgID == sellerID &&
				a.ResourceType == resource.ResourceType && 
				a.ResourceID == resource.ResourceID && 
				a.ResourceParentID == resource.ParentResourceID);
			return assetedResource;
		}

		private List<string> GetAssetIDs(AssetedResourceDO assetedResource, AssetType assetType)
		{
			if (assetType == AssetType.Image) return assetedResource.ImageAssetIDs;
			return assetedResource.AllOtherAssetIDs;
		}

		private string GetPlaceholderImageUrl(ResourceType type)
		{
			var url = $"{_config.BlobStorageHostUrl}/placeholder-images";
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