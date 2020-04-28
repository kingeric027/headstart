using Cosmonaut;
using Cosmonaut.Extensions;
using Marketplace.CMS.Mappers;
using Marketplace.CMS.Models;
using Marketplace.CMS.Storage;
using Marketplace.Common;
using Marketplace.Helpers;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Marketplace.Helpers.Exceptions.ApiErrorException;

namespace Marketplace.CMS.Queries
{
	public interface IAssetQuery
	{
		Task<ListPage<Asset>> List(string containerInteropID, IListArgs args);
		Task<Asset> Get(string containerInteropID, string assetInteropID);
		Task<Asset> Create(string containerInteropID, AssetUploadForm form);
		Task<Asset> Update(string containerInteropID, string assetInteropID, Asset asset);
		Task Delete(string containerInteropID, string assetInteropID);
		Task<ListPage<AssetAssignment>> ListAssignments(string containerInteropID, ListArgs<Asset> args);
		Task SaveAssignment(string containerInteropID, AssetAssignment assignment, VerifiedUserContext user);
		Task DeleteAssignment(string containerInteropID, AssetAssignment assignment);
	}

	public class AssetQuery : IAssetQuery
	{
		private readonly ICosmosStore<Asset> _assetStore;
		private readonly ICosmosStore<AssetAssignment> _assignmentStore;
		private readonly IAssetContainerQuery _containers;
		private readonly IStorageFactory _storageFactory;

		public AssetQuery(ICosmosStore<Asset> assetStore, ICosmosStore<AssetAssignment> assignmentStore, IAssetContainerQuery containers, IStorageFactory storageFactory)
		{
			_assetStore = assetStore;
			_assignmentStore = assignmentStore;
			_containers = containers;
			_storageFactory = storageFactory;
		}

		#region Assets
		public async Task<ListPage<Asset>> List(string containerInteropID, IListArgs args)
		{
			var container = await _containers.Get(containerInteropID);
			var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(container.id) };
			var query = _assetStore.Query(feedOptions)
				.Search(args)
				.Filter(args)
				.Sort(args);
			var list = await query.WithPagination(args.Page, args.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var listPage = list.ToListPage(args.Page, args.PageSize, count);
			listPage.Items = listPage.Items.Select(asset => AssetMapper.MapToResponse(container, asset)).ToList();
			return listPage;
		}

		public async Task<Asset> Get(string containerInteropID, string assetInteropID)
		{
			var container = await _containers.Get(containerInteropID);
			var asset = await GetWithoutExceptions(container.id, assetInteropID);
			if (asset == null) throw new NotFoundException("Asset", assetInteropID);
			return AssetMapper.MapToResponse(container, asset);
		}

		public async Task<Asset> Create(string containerInteropID, AssetUploadForm form)
		{
			var container = await _containers.Get(containerInteropID);
			var (asset, file) = AssetMapper.MapFromUpload(container, form);
			var matchingID = await GetWithoutExceptions(container.id, asset.InteropID);
			if (matchingID != null) throw new DuplicateIdException();
			if (file != null) {			
				await _storageFactory.GetStorage(container).UploadAsset(file, asset);
			}

			var newAsset = await _assetStore.AddAsync(asset);
			return AssetMapper.MapToResponse(container, newAsset);
		}

		public async Task<Asset> Update(string containerInteropID, string assetInteropID, Asset asset)
		{
			var container = await _containers.Get(containerInteropID);
			var existingAsset = await GetWithoutExceptions(container.id, assetInteropID);
			if (existingAsset == null) throw new NotFoundException("Asset", assetInteropID);
			existingAsset.InteropID = asset.InteropID;
			existingAsset.Title = asset.Title;
			existingAsset.Active = asset.Active;
			existingAsset.UrlPathOveride = asset.UrlPathOveride;
			existingAsset.Type = asset.Type;
			existingAsset.Tags = asset.Tags;
			existingAsset.FileName = asset.FileName;
			var updatedAsset = await _assetStore.UpdateAsync(existingAsset);
			return AssetMapper.MapToResponse(container, updatedAsset);
		}

		public async Task Delete(string containerInteropID, string assetInteropID)
		{
			var container = await _containers.Get(containerInteropID); // make sure container exists.
			var asset = await Get(containerInteropID, assetInteropID);
			await _assetStore.RemoveByIdAsync(asset.id, container.id);
			await _storageFactory.GetStorage(container).OnAssetDeleted(asset.id);
		}
		#endregion

		#region Assignments
		public async Task<ListPage<AssetAssignment>> ListAssignments(string containerInteropID, ListArgs<Asset> args)
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
			var assets = await List(container, listPage.Items.Select(assignment => assignment.AssetID));
			for (int i = 0; i < assets.Count; i++)
			{
				listPage.Items[i].Asset = assets[i];
				listPage.Items[i].AssetID = assets[i].InteropID;
			}
			return listPage;
		}

		public async Task SaveAssignment(string containerInteropID, AssetAssignment assignment, VerifiedUserContext user)
		{
			await new MyOrderCloudClient(user).ConfirmExists(assignment.ResourceType, assignment.ResourceID, assignment.ResourceParentID); // confirm OC resource exists
			var asset = await Get(containerInteropID, assignment.AssetID);
			assignment.ContainerID = asset.ContainerID;
			assignment.AssetID = asset.id;
			await _assignmentStore.AddAsync(assignment);
		}

		public async Task DeleteAssignment(string containerInteropID, AssetAssignment assignment)
		{
			var container = await _containers.Get(containerInteropID);
			assignment.ContainerID = container.id;
			await _assignmentStore.RemoveAsync(assignment);
		}
		#endregion

		private async Task<Asset> GetWithoutExceptions(string containerID, string assetInteropID)
		{
			var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(containerID) };
			return await _assetStore.Query($"select top 1 * from c where c.InteropID = @id", new { id = assetInteropID }, feedOptions).FirstOrDefaultAsync();
		}

		private async Task<IList<Asset>> List(AssetContainer container, IEnumerable<string> assetIDs)
		{
			var feedOptions = new FeedOptions() { PartitionKey = new PartitionKey(container.id) };
			var ids = assetIDs.ToList();
			var query = $"select * from c where c.id IN ({string.Join(", ", ids)})";
			var parameters = new ExpandoObject();
			for (int i = 0; i < ids.Count; i++)
			{
				parameters.TryAdd($"id{i}", ids[i]);
			}
			var assets = await _assetStore.QueryMultipleAsync(query, parameters, feedOptions);
			return assets.ToList();
		}
	}
}
