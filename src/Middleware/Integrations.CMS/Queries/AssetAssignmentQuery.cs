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
		Task<AssetAssignment> Get(ResourceType resourceType, string resourceID, string resourceParentID, VerifiedUserContext user);
		Task<ListPage<AssetAssignment>> List(ListArgs<Asset> args, VerifiedUserContext user);
		Task Save(AssetAssignment assignment, VerifiedUserContext user);
		Task Delete(ResourceType resourceType, string resourceID, string resourceParentID, VerifiedUserContext user);
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

		public async Task<ListPage<AssetAssignment>> List(ListArgs<Asset> args, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(container.id) };
			var query = _assignmentStore.Query(feedOptions)
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var listPage = list.ToListPage(args.Page, args.PageSize, count);
			if (count != 0)
			{
				var assetIDs = listPage.Items.Select(assignment => assignment.AssetID);
				var assetDictionary = await _assets.List(container, new HashSet<string>(assetIDs));
				foreach (var assignment in listPage.Items)
				{
					assignment.Asset = assetDictionary[assignment.AssetID];
					assignment.AssetID = assignment.Asset.InteropID;
				}
			}
			return listPage;
		}

		public async Task<AssetAssignment> Get(ResourceType resourceType, string resourceID, string resourceParentID, VerifiedUserContext user)
		{
			var assignment = GetWithoutExceptions(ResourceType resourceType, string resourceID, string resourceParentID);
			if (asset == null) throw new NotFoundException("Asset", assetInteropID);

		}

		public async Task Save(AssetAssignment assignment, VerifiedUserContext user)
		{
			await new MultiTenantOCClient(user).ConfirmExists(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID); // confirm OC resource exists
			var existing = await GetWithoutExceptions(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID);
			var newImages = assignment.ImageAssetIDs.Except(existing.ImageAssetIDs);
			var newOtherAssets = assignment.OtherAssetIDs.Except(existing.OtherAssetIDs);

			await _assignmentStore.AddAsync(assignment);
		}

		public async Task Delete(AssetAssignment assignment, VerifiedUserContext user)
		{
			await _assignmentStore.RemoveAsync(x =>
				x.ResourceID == assignment.ResourceID &&
				x.ResourceType == assignment.ResourceType &&
				x.ResourceParentID == assignment.ResourceParentID);
		}

		private async Task<AssetAssignment> GetWithoutExceptions(ResourceType resourceType, string resourceID, string resourceParentID)
		{
			var parameters = new { 
				ResourceType = resourceType, 
				ResourceID = resourceID, 
				ResourceParentID = resourceParentID
			};
			var query = $"select top 1 * from c where c.ResourceType = @ResourceType && c.ResourceID = @ResourceID && c.ResourceParentID = ResourceParentID";
			return await _assignmentStore.Query(query, parameters).FirstOrDefaultAsync();
		}
	}
}