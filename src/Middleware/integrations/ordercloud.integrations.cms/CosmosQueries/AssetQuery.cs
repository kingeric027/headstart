using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace ordercloud.integrations.cms
{
	public interface IAssetQuery
	{
		Task<ListPage<Asset>> List(ListArgs<Asset> args, VerifiedUserContext user);
		Task<Asset> Get(string assetInteropID, VerifiedUserContext user);
		Task<Asset> Create(AssetUpload form, VerifiedUserContext user);
		Task<Asset> Save(string assetInteropID, Asset asset, VerifiedUserContext user);
		Task Delete(string assetInteropID, VerifiedUserContext user);

		Task<ListPage<AssetDO>> ListByInternalIDs(IEnumerable<string> assetIDs, ListArgsPageOnly args);
		Task<AssetDO> GetDO(string assetInteropID, VerifiedUserContext user);
		Task<AssetDO> GetByInternalID(string assetID); // real id
	}

	public class AssetQuery : IAssetQuery
	{
		private readonly CMSConfig _config;
		private readonly ICosmosStore<AssetDO> _assetStore;
		private readonly IAssetContainerQuery _containers;
		private readonly IBlobStorage _blob;
		private static readonly string[] ValidImageFormats = new[] { "image/png", "image/jpg", "image/jpeg" };

		public AssetQuery(ICosmosStore<AssetDO> assetStore, IAssetContainerQuery containers, IBlobStorage blob, CMSConfig config)
		{
			_assetStore = assetStore;
			_containers = containers;
			_blob = blob;
			_config = config;
		}

		public async Task<ListPage<Asset>> List(ListArgs<Asset> args, VerifiedUserContext user)
		{
			var arguments = args.MapTo();
			var container = await _containers.CreateDefaultIfNotExists(user);
			var query = _assetStore.Query()
				.Where(a => a.ContainerID == container.id)
				.Search(arguments)
				.Filter(arguments)
				.Sort(arguments);
			var list = await query.WithPagination(arguments.Page, arguments.PageSize).ToPagedListAsync();
			var count = await query.CountAsync();
			var assets = list.ToListPage(arguments.Page, arguments.PageSize, count);
			return AssetMapper.MapTo(_config, assets);
		}

		public async Task<Asset> Get(string assetInteropID, VerifiedUserContext user)
		{ 
			return AssetMapper.MapTo(_config, await GetDO(assetInteropID, user));
		}

		public async Task<AssetDO> GetDO(string assetInteropID, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = await GetWithoutExceptions(container.id, assetInteropID);
			if (asset == null) throw new OrderCloudIntegrationException.NotFoundException("Asset", assetInteropID);
			return asset;
		}

		public async Task<Asset> Create(AssetUpload form, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = AssetMapper.MapFromUpload(container, form);
			var matchingID = await GetWithoutExceptions(container.id, asset.InteropID);
			if (matchingID != null) throw new DuplicateIDException();
			if (form.File != null) {
				if (asset.Type == AssetType.Image)
				{
					asset = await OpenImageAndUploadThumbs(container, asset, form);
				}
				await _blob.UploadAsset(container, asset.id, form.File);
			}
			asset.History = HistoryBuilder.OnCreate(user);
			var newAsset = await _assetStore.AddAsync(asset);
			return AssetMapper.MapTo(_config, newAsset);
		}

		public async Task<Asset> Save(string assetInteropID, Asset asset, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			if (assetInteropID != asset.ID)
			{
				var matchingID = await GetWithoutExceptions(container.id, asset.ID);
				if (matchingID != null) throw new DuplicateIDException();
			}
			var existingAsset = await GetWithoutExceptions(container.id, assetInteropID);
			if (existingAsset == null) {
				if (asset.Url == null) throw new AssetCreateValidationException("If you are not uploading a file, you must include a Url");
				existingAsset = new AssetDO()
				{
					Type = asset.Type,
					ContainerID = container.id,
					History = HistoryBuilder.OnCreate(user),
					Metadata = new AssetMetadata() { IsUrlOverridden = true  }
				};
			}
			existingAsset.InteropID = asset.ID;
			existingAsset.Title = asset.Title;
			existingAsset.Active = asset.Active;
			if (existingAsset.Metadata.IsUrlOverridden)
			{
				existingAsset.Url = asset.Url; // Don't allow changing the url if its generated.
			}
			existingAsset.Tags = asset.Tags;
			existingAsset.FileName = asset.FileName;
			existingAsset.History = HistoryBuilder.OnUpdate(existingAsset.History, user);

			// Intentionally don't allow changing the type. Could mess with assignments.
			var updatedAsset = await _assetStore.UpsertAsync(existingAsset);
			return AssetMapper.MapTo(_config, updatedAsset);
		}

		public async Task Delete(string assetInteropID, VerifiedUserContext user)
		{
			var container = await _containers.CreateDefaultIfNotExists(user);
			var asset = await GetDO(assetInteropID, user);
			await _assetStore.RemoveByIdAsync(asset.id, container.id);
			var medium = Task.CompletedTask;
			var small = Task.CompletedTask;
			if (asset.Type == AssetType.Image)
			{
				medium = _blob.DeleteAsset(container, $"{asset.id}-m");
				small = _blob.DeleteAsset(container, $"{asset.id}-s");
			}
			await Task.WhenAll(medium, small, _blob.DeleteAsset(container, asset.id));
		}

		public async Task<ListPage<AssetDO>> ListByInternalIDs(IEnumerable<string> assetIDs, ListArgsPageOnly args)
		{

			return await _assetStore.FindMultipleAsync(assetIDs, args);
		}

		public async Task<AssetDO> GetByInternalID(string assetID)
		{
			var asset = await _assetStore.Query().FirstOrDefaultAsync(a => a.id == assetID);
			if (asset == null) throw new NotImplementedException(); // Why not implemented instead of not found?
			return asset;
		}

		private async Task<AssetDO> OpenImageAndUploadThumbs(AssetContainerDO container, AssetDO asset, AssetUpload form)
		{
			if (!ValidImageFormats.Contains(form.File.ContentType))
			{
				throw new AssetCreateValidationException($"Image Uploads must be one of these file types - {string.Join(", ", ValidImageFormats)}");
			}
			using (var image = Image.FromStream(form.File.OpenReadStream()))
			{
				asset.Metadata.ImageWidth = image.Width;
				asset.Metadata.ImageHeight = image.Height;
				asset.Metadata.ImageHorizontalResolution = (decimal)image.HorizontalResolution;
				asset.Metadata.ImageVerticalResolution = (decimal)image.VerticalResolution;
				var small = image.ResizeSmallerDimensionToTarget(100);
				var medium = image.ResizeSmallerDimensionToTarget(300);
				await Task.WhenAll(new[] {
					_blob.UploadAsset(container, $"{asset.id}-m", medium),
					_blob.UploadAsset(container, $"{asset.id}-s", small)
				});
			}
			return asset;
		}

		private async Task<AssetDO> GetWithoutExceptions(string containerID, string assetInteropID)
		{
			var asset = await _assetStore.Query().FirstOrDefaultAsync(a => a.InteropID == assetInteropID && a.ContainerID == containerID);
			return asset;
		}
 	}
}
