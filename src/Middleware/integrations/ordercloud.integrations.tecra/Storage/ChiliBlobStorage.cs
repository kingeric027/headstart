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
		Task<string> UploadAsset(string blobName, byte[] bytes);
		Task<string> UploadAsset(string blobName, Image image);
	}

	public class ChiliBlobStorage : IChiliBlobStorage
	{
		private readonly CMSConfig _config;
		private readonly IOrderCloudIntegrationsBlobService _blob;
		string chiliContainer = "chili-assets";

		public ChiliBlobStorage(CMSConfig config)
		{
			_config = config;
			_blob = new OrderCloudIntegrationsBlobService(new BlobServiceConfig()
			{
				ConnectionString = _config.BlobStorageConnectionString,
				Container = chiliContainer,
				AccessType = BlobContainerPublicAccessType.Container
			});
		}


		public async Task<string> UploadAsset(string blobName, byte[] bytes)
		{
			try
			{
				await _blob.Save(blobName, bytes, "application/pdf");
				return _config.BlobStorageHostUrl + "/" + chiliContainer + "/" + blobName;
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(chiliContainer, ex);
			}
		}
		public async Task<string> UploadAsset(string blobName, Image image)
		{
			try
			{
				var bytes = image.ToBytes(ImageFormat.Png);
				await _blob.Save(blobName, bytes, "image/png");
				return _config.BlobStorageHostUrl + "/" + chiliContainer + "/" + blobName;
			}
			catch (Exception ex)
			{
				throw new StorageConnectionException(chiliContainer, ex);
			}
		}
	}
}
