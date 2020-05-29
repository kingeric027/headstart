using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
#if NETCOREAPP2_2
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif
#if NETCOREAPP3_1
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
#endif

namespace ordercloud.integrations.library
{
    public interface IOrderCloudIntegrationsBlobService
    {
		Task<T> Get<T>(string id);
        Task<string> Get(string id);
        Task Save(string reference, string blob, string fileType = null);
#if NETCOREAPP3_1
        Task Save(string reference, JsonDocument blob, string fileType = null);
#endif
#if NETCOREAPP2_2
        Task Save(string reference, JObject blob, string fileType = null);
#endif
        Task Save(string reference, IFormFile blob, string fileType = null);
        Task Save(string reference, byte[] bytes, string fileType = null);
        Task Save(BlobBase64Image base64Image);
        Task Delete(string id);
		Task DeleteContainer();

	}
	public class OrderCloudIntegrationsBlobService : IOrderCloudIntegrationsBlobService
    {
        public CloudBlobContainer Container { get;  }
		private readonly BlobServiceConfig _config;

		// BlobServiceConfig must be required for this service to function properly
		public OrderCloudIntegrationsBlobService() : this(new BlobServiceConfig())
        {

        }

        public OrderCloudIntegrationsBlobService(BlobServiceConfig config)
        {
			_config = config;
            try
            {
                if (config.ConnectionString == null)
                    throw new Exception("Connection string not supplied");
                if (config.Container == null)
                    throw new Exception("Blob container not specified");

                CloudStorageAccount.TryParse(config.ConnectionString, out var storage);
                var client = storage.CreateCloudBlobClient();
                if (config.Container != null)
                    Container = client.GetContainerReference(config.Container);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}. The blob service must be invoked with a valid configuration");
            }
        }
		private async Task Init()
		{
			var created = await Container.CreateIfNotExistsAsync();
			if (created)
			{
				var permissions = await Container.GetPermissionsAsync();
				permissions.PublicAccess = _config.AccessType;
				await Container.SetPermissionsAsync(permissions);
			}
		}

		public async Task<string> Get(string id)
        {
            await this.Init();
            var value = await Container.GetBlockBlobReference(id).DownloadTextAsync();
            return value;
        }

#if NETCOREAPP3_1
        public virtual async Task<T> Get<T>(string id)
        {
            await this.Init();
            var obj = await Container.GetBlockBlobReference(id).DownloadTextAsync();
            return JsonSerializer.Deserialize<T>(obj);
        }

        public async Task Save(string reference, JsonDocument blob, string fileType = null)
        {
            await this.Init();
            var block = Container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadTextAsync(JsonSerializer.Serialize(blob), Encoding.UTF8, AccessCondition.GenerateEmptyCondition(),
                new BlobRequestOptions() { SingleBlobUploadThresholdInBytes = 4 * 1024 * 1024 }, new OperationContext());
        }
#endif
#if NETCOREAPP2_2
        public virtual async Task<T> Get<T>(string id)
        {
            await this.Init();
            var obj = await Container.GetBlockBlobReference(id).DownloadTextAsync();
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public async Task Save(string reference, JObject blob, string fileType = null)
        {
            await this.Init();
            var block = Container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadTextAsync(JsonConvert.SerializeObject(blob), Encoding.UTF8, AccessCondition.GenerateEmptyCondition(),
                new BlobRequestOptions() { SingleBlobUploadThresholdInBytes = 4 * 1024 * 1024 }, new OperationContext());
        }
#endif
        public async Task Save(string reference, byte[] bytes, string fileType = null)
        {
            await this.Init();
            var block = Container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
        }

        public async Task Save(string reference, string blob, string fileType = null)
        {
            await this.Init();
            var block = Container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadTextAsync(blob, Encoding.UTF8, AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions() { SingleBlobUploadThresholdInBytes = 4 * 1024 * 1024 }, new OperationContext());
        }

        public async Task Save(string reference, IFormFile blob, string fileType = null)
        {
            var block = Container.GetBlockBlobReference(reference);
            block.Properties.ContentType = fileType ?? blob.ContentType;
#if NETCOREAPP3_1
            await using var stream = blob.OpenReadStream();
            await block.UploadFromStreamAsync(stream);
#endif
#if NETCOREAPP2_2
            using var stream = blob.OpenReadStream();
            await block.UploadFromStreamAsync(stream);
#endif
        }

        public async Task Save(BlobBase64Image base64Image)
        {
            await this.Init();
            var block = Container.GetBlockBlobReference(base64Image.Reference);
            block.Properties.ContentType = base64Image.ContentType;
            await block.UploadFromByteArrayAsync(base64Image.Bytes, 0, base64Image.Bytes.Length);
        }

        public async Task Delete(string id)
        {
            await this.Init();
            await Container.GetBlockBlobReference(id).DeleteIfExistsAsync();
        }

		public async Task DeleteContainer()
		{
			await Container.DeleteAsync();
		}
	}
}
