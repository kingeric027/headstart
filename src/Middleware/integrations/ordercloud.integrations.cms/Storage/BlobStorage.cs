using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Drawing;
using System.Drawing.Imaging;

namespace ordercloud.integrations.cms
{
	public interface IBlobStorage
	{
		Task UploadAsset(AssetContainerDO container, string blobName, IFormFile file);
		Task UploadAsset(AssetContainerDO container, string blobName, Image image);
		Task DeleteAsset(AssetContainerDO container, string blobName);
	}

	public class BlobStorage : IBlobStorage
	{
		private readonly CMSConfig _config;

		public BlobStorage(CMSConfig config)
		{
			_config = config;
		}


		public async Task UploadAsset(AssetContainerDO container, string blobName, IFormFile file)
		{
			try
			{
				await BuildBlobService(container).Save(blobName, file);
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.id, ex);
			}
		}

		public async Task UploadAsset(AssetContainerDO container, string blobName, Image image)
		{
			try
			{
				var bytes = image.ToBytes(ImageFormat.Png);
				await BuildBlobService(container).Save(blobName, bytes, "image/png");
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.id, ex);
			}
		}

		public async Task DeleteAsset(AssetContainerDO container, string blobName)
		{
			try
			{
				await BuildBlobService(container).Delete(blobName);
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container.id, ex);
			}
		}

		private OrderCloudIntegrationsBlobService BuildBlobService(AssetContainerDO container)
		{
			return new OrderCloudIntegrationsBlobService(new BlobServiceConfig()
			{
				ConnectionString = _config.BlobStorageConnectionString,
				Container = $"assets-{container.id}", // SellerOrgID can contain "_", an illegal character for blob containers.
				AccessType = BlobContainerPublicAccessType.Container
			});
		}
	}
}
