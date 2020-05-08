using Cosmonaut;
using Cosmonaut.Extensions;
using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			await new MultiTenantOCClient(user).Get(resource);
			var assetedResource = await GetExisting(resource);
			if (assetedResource == null) return new List<AssetForDelivery>();
			var assetIDs = assetedResource.ImageAssetIDs.Concat(assetedResource.OtherAssetIDs).ToList();

			var assets = await _assets.ListAcrossContainers(assetIDs);
			return assets.Select(asset => {
				int indexWithinType;
				switch (asset.Type)
				{
					case AssetType.Image:
						indexWithinType = assetedResource.ImageAssetIDs.IndexOf(asset.id);
						break;
					default:
						indexWithinType = assetedResource.OtherAssetIDs.IndexOf(asset.id);
						break;
				}
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
			switch (asset.Type)
			{
				case AssetType.Image:
					if (!assetedResource.ImageAssetIDs.Contains((asset.id))) assetedResource.ImageAssetIDs.Add(asset.id); // check if already exists
					break;
				default:
					if (!assetedResource.OtherAssetIDs.Contains((asset.id))) assetedResource.OtherAssetIDs.Add(asset.id);
					break;
			}
			await _store.UpsertAsync(assetedResource);
		}

		public async Task DeleteAssignment(Resource resource, string assetID, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).EmptyPatch(resource);
			var asset = await _assets.Get(assetID, user);
			var assetedResource = await GetExistingOrDefault(resource);
			switch (asset.Type)
			{
				case AssetType.Image:
					assetedResource.ImageAssetIDs.Remove(asset.id);
					break;
				default:
					assetedResource.OtherAssetIDs.Remove(asset.id);
					break;
			}
			await _store.UpdateAsync(assetedResource);
		}

		public async Task MoveAssignment(Resource resource, string assetID, int listOrderWithinType, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).EmptyPatch(resource);
			var asset = await _assets.Get(assetID, user);
			var assetedResource = await GetExistingOrDefault(resource);
			switch (asset.Type)
			{
				case AssetType.Image:
					assetedResource.ImageAssetIDs.Remove(asset.id);
					var index = Math.Min(listOrderWithinType, assetedResource.ImageAssetIDs.Count);
					assetedResource.ImageAssetIDs.Insert(index, asset.id);
					break;
				default:
					assetedResource.OtherAssetIDs.Remove(asset.id);
					index = Math.Min(listOrderWithinType, assetedResource.ImageAssetIDs.Count);
					assetedResource.OtherAssetIDs.Insert(index, asset.id);
					break;
			}
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
	}
}