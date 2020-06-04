using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	public interface IBlobStorage
	{
		CMSConfig Config { get; }
		Task<Asset> UploadAsset(AssetContainer container, IFormFile file, Asset asset);
		Task OnContainerDeleted(AssetContainer container);
		Task OnAssetDeleted(AssetContainer container, string assetID);
	}

	public class BlobStorage : IBlobStorage
	{
		public CMSConfig Config { get; }

		public BlobStorage(CMSConfig config)
		{
			Config = config;
		}

		public async Task<Asset> UploadAsset(AssetContainer container, IFormFile file, Asset asset)
		{
			try
			{
				await BuildBlobService(container).Save(asset.id, file);
				return asset;
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
		}

		public async Task OnContainerDeleted(AssetContainer container)
		{
			try
			{
				await BuildBlobService(container).DeleteContainer();
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
		}

		public async Task OnAssetDeleted(AssetContainer container, string assetID)
		{
			try
			{
				await BuildBlobService(container).Delete(assetID);
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
		}

		private OrderCloudIntegrationsBlobService BuildBlobService(AssetContainer container)
		{
			return new OrderCloudIntegrationsBlobService(new BlobServiceConfig()
			{
				ConnectionString = Config.BlobStorageConnectionString,
				Container = $"assets-{container.id}",
				AccessType = BlobContainerPublicAccessType.Container
			});
		}
	}
}
