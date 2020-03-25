using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Helpers.Services
{
    public class BlobServiceConfig
    {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
    }

    public interface IBlobService
    {
        Task<T> Get<T>(string id);
        Task<string> Get(string id);
        Task Save(string reference, string blob, string fileType = null);
        Task Save(string reference, JObject blob, string fileType = null);
		Task Save(string reference, IFormFile blob, string fileType = null);
        Task Save(string reference, byte[] bytes, string fileType = null)
        Task Save(BlobBase64Image base64Image);
        Task Delete(string id);
    }

    public class BlobService : IBlobService
    {
        private readonly CloudBlobContainer _container;

        // BlobServiceConfig must be required for this service to function properly
        public BlobService() : this(new BlobServiceConfig())
        {

        }

        public BlobService(BlobServiceConfig config)
        {
            try
            {
                if (config.ConnectionString == null)
                    throw new Exception("Connection string not supplied");
                if (config.Container == null)
                    throw new Exception("Blob container not specified");

                CloudStorageAccount.TryParse(config.ConnectionString, out var storage);
                var client = storage.CreateCloudBlobClient();
                if (config.Container != null)
                    _container = client.GetContainerReference(config.Container);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}. The blob service must be invoked with a valid configuration");
            }
        }

        private async Task Init()
        {
            await _container.CreateIfNotExistsAsync();
        }

        public async Task<string> Get(string id)
        {
            await this.Init();
            var value = await _container.GetBlockBlobReference(id).DownloadTextAsync();
            return value;
        }

        public async Task<T> Get<T>(string id)
        {
            await this.Init();
            var obj = await _container.GetBlockBlobReference(id).DownloadTextAsync();
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public async Task Save(string reference, JObject blob, string fileType = null)
        {
            await this.Init();
            var block = _container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadTextAsync(JsonConvert.SerializeObject(blob), Encoding.UTF8, AccessCondition.GenerateEmptyCondition(),
                new BlobRequestOptions() { SingleBlobUploadThresholdInBytes = 4 * 1024 * 1024 }, new OperationContext());
        }

        public async Task Save(string reference, byte[] bytes, string fileType = null)
        {
            await this.Init();
            var block = _container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }

        public async Task Save(string reference, string blob, string fileType = null)
        {
            await this.Init();
            var block = _container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadTextAsync(blob, Encoding.UTF8, AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions() { SingleBlobUploadThresholdInBytes = 4 * 1024 * 1024 }, new OperationContext());
        }

		public async Task Save(string reference, IFormFile blob, string fileType = null)
		{
			var block = _container.GetBlockBlobReference(reference);
			block.Properties.ContentType = fileType ?? blob.ContentType;
			using (var stream = blob.OpenReadStream())
			{
				await block.UploadFromStreamAsync(stream);
			}
		}

		public async Task Save(BlobBase64Image base64Image)
        {
            await this.Init();
            var block = _container.GetBlockBlobReference(base64Image.Reference);
            block.Properties.ContentType = base64Image.ContentType;
            await block.UploadFromByteArrayAsync(base64Image.Bytes, 0, base64Image.Bytes.Length);
        }

        public async Task Delete(string id)
        {
            await this.Init();
            await _container.GetBlockBlobReference(id).DeleteIfExistsAsync();
        }
    }
}
