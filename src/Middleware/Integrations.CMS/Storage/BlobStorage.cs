using Integrations.CMS;
using Marketplace.CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.blob;

namespace Marketplace.CMS.Storage
{
	public interface IBlobStorage
	{
		CMSConfig Config { get; }
		Task<AssetContainer> OnContainerConnected(AssetContainer container);
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

		public async Task<AssetContainer> OnContainerConnected(AssetContainer container)
		{
			try
			{
				//TODO: work with Oliver on this merge
				// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-manage-access-to-resources
				//await BuildBlobService(container).Init(BlobContainerPublicAccessType.Container);
				//return container;
				return new AssetContainer();
			} catch (Exception ex)
			{
				throw new StorageConnectionException(container.InteropID, ex);
			}
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
				Container = $"assets-{container.id}"
			});
		}
	}
}
