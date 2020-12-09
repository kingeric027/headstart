using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ordercloud.integrations.tecra.Storage
{
	public interface IChiliBlobStorage
    {
		Task<string> UploadAsset(string blobName, byte[] bytes, string fileType);
    }

	public class ChiliBlobStorage : IChiliBlobStorage
	{
		private readonly OrderCloudTecraConfig _config;
		private readonly IOrderCloudIntegrationsBlobService _blob;
		string chiliContainer = "chili-assets";

		public ChiliBlobStorage(OrderCloudTecraConfig config)
		{
			_config = config;
			_blob = new OrderCloudIntegrationsBlobService(new BlobServiceConfig()
			{
				ConnectionString = _config.BlobStorageConnectionString,
				Container = chiliContainer,
				AccessType = BlobContainerPublicAccessType.Container
			});
		}

		public async Task<string> UploadAsset(string blobName, byte[] bytes, string fileType)
		{
			try
			{
				await _blob.Save(blobName, bytes, fileType);
				return _config.BlobStorageHostUrl + "/" + chiliContainer + "/" + blobName;
			}
			catch (Exception ex)
			{
				throw new Exception($"Container - {chiliContainer}. {ex}");
			}
		}
	}
}
