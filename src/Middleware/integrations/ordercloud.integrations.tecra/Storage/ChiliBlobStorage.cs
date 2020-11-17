using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using ordercloud.integrations.cms;
using System.Drawing;
using System.Drawing.Imaging;

namespace ordercloud.integrations.tecra.Storage
{
	public interface IChiliBlobStorage
	{
		Task<string> UploadAsset(string container, string blobName, byte[] bytes);
		Task<string> UploadAsset(string container, string blobName, Image image);
	}

	public class ChiliBlobStorage : IChiliBlobStorage
	{
		private readonly CMSConfig _config;

		public ChiliBlobStorage(CMSConfig config)
		{
			_config = config;
		}


		public async Task<string> UploadAsset(string container, string blobName, byte[] bytes)
		{
			try
			{
				await BuildBlobService(container).Save(blobName, bytes, "application/pdf");
				return _config.BlobStorageHostUrl + "/" + container + "/" + blobName;
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container, ex);
			}
		}
		public async Task<string> UploadAsset(string container, string blobName, Image image)
		{
			try
			{
				var bytes = image.ToBytes(ImageFormat.Png);
				await BuildBlobService(container).Save(blobName, bytes, "image/png");
				return _config.BlobStorageHostUrl + "/" + container + "/" + blobName;
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(container, ex);
			}
		}

		private OrderCloudIntegrationsBlobService BuildBlobService(string container)
		{
			return new OrderCloudIntegrationsBlobService(new BlobServiceConfig()
			{
				ConnectionString = _config.BlobStorageConnectionString,
				Container = container,
				AccessType = BlobContainerPublicAccessType.Container
			});
		}
	}
}
