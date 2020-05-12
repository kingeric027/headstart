using Cosmonaut;
using Cosmonaut.Extensions;
using Integrations.CMS;
using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ordercloud.integrations.extensions;
using Resource = Marketplace.CMS.Models.Resource;

namespace Marketplace.CMS.Queries
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

		public AssetedResourceQuery(ICosmosStore<AssetedResource> store, IAssetQuery assets)
		{
			_store = store;
			_assets = assets;
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
			if (assetedResource == null) return "No Image";
			if (assetedResource.ImageAssetIDs.Count > 0)
			{
				var asset = await _assets.GetAcrossContainers(assetedResource.ImageAssetIDs.First());
				return asset.Url;
			} else
			{
				throw new NotImplementedException();
			}
		}

		public async Task SaveAssignment(Resource resource, string assetID, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).EmptyPatch(resource);
			var asset = await _assets.Get(assetID, user);
			var assetedResource = await GetExistingOrDefault(resource);
			GetAssetIDs(assetedResource, asset.Type).UniqueAdd(asset.id); 
			await _store.UpsertAsync(assetedResource);
		}

		public async Task DeleteAssignment(Resource resource, string assetID, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).EmptyPatch(resource);
			var asset = await _assets.Get(assetID, user);
			var assetedResource = await GetExistingOrDefault(resource);
			GetAssetIDs(assetedResource, asset.Type).Remove(asset.id);
			await _store.UpdateAsync(assetedResource);
		}

		public async Task MoveAssignment(Resource resource, string assetID, int listOrderWithinType, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).EmptyPatch(resource);
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

		public List<string> GetAssetIDs(AssetedResource assetedResource, AssetType assetType)
		{
			var property = assetedResource.GetType().GetProperty($"{assetType.ToString()}AssetIDs");
			var list = (List<string>) property.GetValue(assetedResource, null) ?? new List<string>();
			return list;
		}
	}
}