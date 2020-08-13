using System;
using System.Collections.Generic;
using System.Linq;
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
		Task<string> GetFirstImage(Resource resource, VerifiedUserContext user);
		Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task MoveAssignment(AssetAssignment assignment, int listOrderWithinType, VerifiedUserContext user);
	}

	public class AssetedResourceQuery : IAssetedResourceQuery
	{
		private readonly ICosmosStore<AssetedResourceDO> _store;
		private readonly IAssetQuery _assets;
		private readonly IBlobStorage _blob;

		public AssetedResourceQuery(ICosmosStore<AssetedResourceDO> store, IAssetQuery assets, IBlobStorage blob)
		{
			_store = store;
			_assets = assets;
			_blob = blob;
		}

		public async Task<ListPage<Asset>> ListAssets(Resource resource, ListArgsPageOnly args, VerifiedUserContext user)
		{
			resource.Validate();
			args = args ?? new ListArgsPageOnly();
			// Confirm user has access to resource.
			// await new MultiTenantOCClient(user).Get(resource); Commented out until I solve visiblity for /me endpoints
			var assetedResource = await GetExisting(resource);
			if (assetedResource == null) return new ListPage<Asset>();
			var assetIDs = assetedResource.ImageAssetIDs
				.Concat(assetedResource.ThemeAssetIDs)
				.Concat(assetedResource.AttachmentAssetIDs)
				.Concat(assetedResource.StructuredAssetsIDs)
				.ToList();
			var assets = await _assets.ListByInternalIDs(assetIDs, args);
			var items = assets.Items.Select(a => {
				int indexWithinType = GetAssetIDs(assetedResource, a.Type).IndexOf(a.id);
				var asset = AssetMapper.MapTo(a);
				return (asset, indexWithinType);
			}).OrderBy(c => c.asset.Type).ThenBy(c => c.indexWithinType).Select(x => x.asset).ToList();
			return new ListPage<Asset>()
			{
				Items = items,
				Meta = assets.Meta
			};
		}

		public async Task<string> GetFirstImage(Resource resource, VerifiedUserContext user)
		{
			resource.Validate();
			var assetedResource = await GetExisting(resource);
			if (assetedResource?.ImageAssetIDs == null || assetedResource.ImageAssetIDs.Count == 0)
			{
				return GetPlaceholderImageUrl(resource.ResourceType ?? 0);
			}
			var asset = await _assets.GetByInternalID(assetedResource.ImageAssetIDs.First());
			return asset.Url;
		}

		public async Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			assignment.Validate();
			await new OrderCloudClientWithContext(user).EmptyPatch(assignment);
			var asset = await _assets.GetDO(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment);
			GetAssetIDs(assetedResource, asset.Type).UniqueAdd(asset.id); 
			await _store.UpsertAsync(assetedResource);
		}

		public async Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			assignment.Validate();
			await new OrderCloudClientWithContext(user).EmptyPatch(assignment);
			var asset = await _assets.GetDO(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment);
			GetAssetIDs(assetedResource, asset.Type).Remove(asset.id);
			await _store.UpdateAsync(assetedResource);
		}

		public async Task MoveAssignment(AssetAssignment assignment, int listOrderWithinType, VerifiedUserContext user)
		{
			assignment.Validate();
			await new OrderCloudClientWithContext(user).EmptyPatch(assignment);
			var asset = await _assets.GetDO(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment);
			GetAssetIDs(assetedResource, asset.Type).MoveTo(asset.id, listOrderWithinType);
			await _store.UpdateAsync(assetedResource);
		}

		private async Task<AssetedResourceDO> GetExistingOrDefault(Resource resource)
		{
			return await GetExisting(resource) ?? new AssetedResourceDO() { 
				ResourceID = resource.ResourceID, 
				ResourceParentID = resource.ParentResourceID, 
				ResourceType = resource.ResourceType ?? 0 // "Required" validation should prevent null ResourceType
		};
		}

		private async Task<AssetedResourceDO> GetExisting(Resource resource)
		{
			var assetedResource = await _store.Query().FirstOrDefaultAsync(
				a => a.ResourceType == resource.ResourceType && 
				a.ResourceID == resource.ResourceID && 
				a.ResourceParentID == resource.ParentResourceID);
			return assetedResource;
		}

		private List<string> GetAssetIDs(AssetedResourceDO assetedResource, AssetType assetType)
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