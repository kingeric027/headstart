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
	public interface IAssetAssignmentQuery
	{
		Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user);
		Task MoveAssignment(AssetAssignment assignment, int position, VerifiedUserContext user);
	}

	public class AssetAssignmentQuery : IAssetAssignmentQuery
	{
		private readonly ICosmosStore<AssetedResource> _store;
		private readonly IAssetQuery _assets;
		private readonly IAssetContainerQuery _containers;

		public AssetAssignmentQuery(ICosmosStore<AssetedResource> store, IAssetQuery assets, IAssetContainerQuery containers)
		{
			_store = store;
			_containers = containers;
			_assets = assets;
		}

		//public async Task<ListPage<AssetAssignment>> List(ListArgs<Asset> args, VerifiedUserContext user)
		//{
		//	var container = await _containers.CreateDefaultIfNotExists(user);
		//	var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(container.id) };
		//	var query = _assignmentStore.Query(feedOptions)
		//		.Search(args)
		//		.Filter(args)
		//		.Sort(args);
		//	var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
		//	var count = await query.CountAsync();
		//	var listPage = list.ToListPage(args.Page, args.PageSize, count);
		//	if (count != 0)
		//	{
		//		var assetIDs = listPage.Items.Select(assignment => assignment.AssetID);
		//		var assetDictionary = await _assets.List(container, new HashSet<string>(assetIDs));
		//		foreach (var assignment in listPage.Items)
		//		{
		//			assignment.Asset = assetDictionary[assignment.AssetID];
		//			assignment.AssetID = assignment.Asset.InteropID;
		//		}
		//	}
		//	return listPage;
		//}

		public async Task SaveAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			var asset = await _assets.Get(assignment.AssetID, user);
			var assetedResource = await GetWithoutExceptions(assignment);
			switch (asset.Type)
			{
				case AssetType.Image:
					if (!assetedResource.ImageAssetIDs.Contains((asset.id))) assetedResource.ImageAssetIDs.Add(asset.id); // check if already exists
					break;
				default:
					if (!assetedResource.ImageAssetIDs.Contains((asset.id))) assetedResource.ImageAssetIDs.Add(asset.id);
					break;
			}
			await _store.UpdateAsync(assetedResource);
		}

		public async Task DeleteAssignment(AssetAssignment assignment, VerifiedUserContext user)
		{
			var asset = await _assets.Get(assignment.AssetID, user);
			var assetedResource = await GetWithoutExceptions(assignment);
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


		public async Task MoveAssignment(AssetAssignment assignment, int position, VerifiedUserContext user)
		{
			var asset = await _assets.Get(assignment.AssetID, user);
			var assetedResource = await GetWithoutExceptions(assignment);
			switch (asset.Type)
			{
				case AssetType.Image:
					assetedResource.ImageAssetIDs.Remove(asset.id);
					assetedResource.ImageAssetIDs.Insert(position, asset.id);
					break;
				default:
					assetedResource.ImageAssetIDs.Remove(asset.id);
					assetedResource.ImageAssetIDs.Insert(position, asset.id); ;
					break;
			}
			await _store.UpdateAsync(assetedResource);
		}

		private async Task<AssetedResource> GetWithoutExceptions(AssetAssignment assignment)
		{
			return await GetWithoutExceptions(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID);
		}

		private async Task<AssetedResource> GetWithoutExceptions(ResourceType resourceType, string resourceID, string resourceParentID)
		{
			var parameters = new { 
				ResourceType = resourceType, 
				ResourceID = resourceID, 
				ResourceParentID = resourceParentID
			};
			var query = $"select top 1 * from c where c.ResourceType = @ResourceType && c.ResourceID = @ResourceID && c.ResourceParentID = ResourceParentID";
			return await _store.Query(query, parameters).FirstOrDefaultAsync();
		}
	}
}