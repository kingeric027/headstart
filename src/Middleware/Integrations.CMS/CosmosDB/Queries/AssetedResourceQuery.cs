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

namespace Marketplace.CMS.Queries
{
	public interface IAssetedResourceQuery
	{
		Task<List<AssetForDelivery>> DeliverAssets(ResourceType type, string resourceID, string resourceParentID, VerifiedUserContext user);
		Task<string> DeliverFirstImageUrl(ResourceType type, string resourceID, string resourceParentID, VerifiedUserContext user);
		Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task MoveAssignment(AssetAssignment assignment, int listOrder, VerifiedUserContext user);
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

		public async Task<List<AssetForDelivery>> DeliverAssets(ResourceType type, string resourceID, string resourceParentID, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).Get(type, resourceID, resourceParentID);
			var assetedResource = await GetExistingOrDefault(type, resourceID, resourceParentID);
			var assetIDs = assetedResource.ImageAssetIDs.Concat(assetedResource.OtherAssetIDs).ToList();		

			var assets = await _assets.ListAcrossContainers(assetIDs);
			return assets.Select(a => new AssetForDelivery(a, assetIDs.IndexOf(a.id))).OrderBy(a => a.ListOrder).ToList();
		}

		public async Task<string> DeliverFirstImageUrl(ResourceType type, string resourceID, string resourceParentID, VerifiedUserContext user)
		{
			var assetedResource = await GetExistingOrDefault(type, resourceID, resourceParentID);
			if (assetedResource.ImageAssetIDs.Count > 0)
			{
				var asset = await _assets.GetAcrossContainers(assetedResource.ImageAssetIDs.First());
				return asset.Url;
			} else
			{
				throw new NotImplementedException();
			}
		}

		public async Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).Get(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID);
			var asset = await _assets.Get(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment);
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

		public async Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).Get(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID);
			var asset = await _assets.Get(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment);
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


		public async Task MoveAssignment(AssetAssignment assignment, int listOrder, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).Get(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID);
			var asset = await _assets.Get(assignment.AssetID, user);
			var assetedResource = await GetExistingOrDefault(assignment);
			switch (asset.Type)
			{
				case AssetType.Image:
					assetedResource.ImageAssetIDs.Remove(asset.id);
					assetedResource.ImageAssetIDs.Insert(listOrder, asset.id);
					break;
				default:
					assetedResource.OtherAssetIDs.Remove(asset.id);
					assetedResource.OtherAssetIDs.Insert(listOrder, asset.id); ;
					break;
			}
			await _store.UpdateAsync(assetedResource);
		}

		private async Task<AssetedResource> GetExistingOrDefault(AssetAssignment assignment)
		{
			return await GetExistingOrDefault(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID);
		}

		private async Task<AssetedResource> GetExistingOrDefault(ResourceType resourceType, string resourceID, string resourceParentID)
		{
			var parameters = new { 
				ResourceType = resourceType, 
				ResourceID = resourceID,
				ResourceParentID = resourceParentID
			};
			var query = $"select top 1 * from c where c.ResourceType = @ResourceType AND c.ResourceID = @ResourceID AND c.ResourceParentID = @ResourceParentID";
			var resource = await _store.QuerySingleAsync(query, parameters);
			return resource ?? new AssetedResource()
			{
				ResourceType = resourceType,
				ResourceID = resourceID,
				ResourceParentID = resourceParentID
			};
		}
	}
}