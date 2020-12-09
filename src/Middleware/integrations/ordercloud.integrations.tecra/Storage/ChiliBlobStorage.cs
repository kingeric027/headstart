using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using ordercloud.integrations.cms;
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
        private const string chiliContainer = "chili-assets";

        public ChiliBlobStorage(OrderCloudTecraConfig config, IOrderCloudIntegrationsBlobService blob)
		{
			_config = config;
            _blob = blob;
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
				throw new StorageConnectionException(chiliContainer, ex);
			}
		}
	}
}
