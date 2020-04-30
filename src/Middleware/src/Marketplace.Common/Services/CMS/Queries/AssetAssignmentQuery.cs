using Cosmonaut;
using Cosmonaut.Extensions;
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
		Task<ListPage<AssetAssignment>> List(string containerInteropID, ListArgs<Asset> args);
		Task Save(string containerInteropID, AssetAssignment assignment, VerifiedUserContext user);
		Task Delete(string containerInteropID, string assetInteropID, ResourceType resourceType, string resourceID);
	}

	public class AssetAssignmentQuery : IAssetAssignmentQuery
	{
		private readonly ICosmosStore<AssetAssignment> _assignmentStore;
		private readonly IAssetQuery _assets;
		private readonly IAssetContainerQuery _containers;

		public AssetAssignmentQuery(ICosmosStore<AssetAssignment> assignmentStore, IAssetQuery assets, IAssetContainerQuery containers)
		{
			_assignmentStore = assignmentStore;
			_containers = containers;
			_assets = assets;
		}

		public async Task<ListPage<AssetAssignment>> List(string containerInteropID, ListArgs<Asset> args)
		{
			var container = await _containers.Get(containerInteropID);
			var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(container.id) };
			var query = _assignmentStore.Query(feedOptions)
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var listPage = list.ToListPage(args.Page, args.PageSize, count);
			var assetIDs = listPage.Items.Select(assignment => assignment.AssetID);
			var assetDictionary = await _assets.List(container, new HashSet<string>(assetIDs));
			foreach (var assignment in listPage.Items)
			{
				assignment.Asset = assetDictionary[assignment.AssetID];
				assignment.AssetID = assignment.Asset.InteropID;
			}
			return listPage;
		}

		public async Task Save(string containerInteropID, AssetAssignment assignment, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).ConfirmExists(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID); // confirm OC resource exists
			var asset = await _assets.Get(containerInteropID, assignment.AssetID);
			assignment.ContainerID = asset.ContainerID;
			assignment.AssetID = asset.id;
			await _assignmentStore.AddAsync(assignment);
		}

		public async Task Delete(string containerInteropID, string assetInteropID, ResourceType resourceType, string resourceID)
		{
			var container = await _containers.Get(containerInteropID);
			var asset = await _assets.Get(containerInteropID, assetInteropID);
			await _assignmentStore.RemoveAsync(x =>
				x.ContainerID == container.id &&
				x.AssetID == asset.id &&
				x.ResourceID == resourceID &&
				x.ResourceType == resourceType
			);
		}
	}
}