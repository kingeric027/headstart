using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Services
{
    public interface IBlobService
    {
        Task<T> Get<T>(string container, string id);
        Task<string> Get(string container, string id);
        Task Save(string container, string reference, string blob, string fileType = null);
        Task Save(string container, string reference, JObject blob, string fileType = null);
        Task Delete(string container, string id);
    }

    public class BlobService : IBlobService
    {
        private readonly CloudStorageAccount _storage;
        private CloudBlobContainer _container;
        private CloudBlobClient _client;

        public BlobService(IAppSettings settings)
        {
            CloudStorageAccount.TryParse(settings.BlobSettings.ConnectionString, out _storage);
        }

        private async Task Init(string container)
        {
            _client = _storage.CreateCloudBlobClient();
            _container = _client.GetContainerReference(container);
            await _container.CreateIfNotExistsAsync();
        }

        public async Task<string> Get(string container, string id)
        {
            await this.Init(container);
            var value = await _container.GetBlockBlobReference(id).DownloadTextAsync();
            return value;
        }

        public async Task<T> Get<T>(string container, string id)
        {
            await this.Init(container);
            var obj = await _container.GetBlockBlobReference(id).DownloadTextAsync();
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public async Task Save(string container, string reference, JObject blob, string fileType = null)
        {
            await this.Init(container);
            var block = _container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadTextAsync(JsonConvert.SerializeObject(blob), Encoding.UTF8, AccessCondition.GenerateEmptyCondition(), 
                new BlobRequestOptions() { SingleBlobUploadThresholdInBytes = 4 * 1024 * 1024 }, new OperationContext());
        }

        public async Task Save(string container, string reference, string blob, string fileType = null)
        {
            await this.Init(container);
            var block = _container.GetBlockBlobReference(reference);
            if (fileType != null)
                block.Properties.ContentType = fileType;
            await block.UploadTextAsync(blob, Encoding.UTF8, AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions() { SingleBlobUploadThresholdInBytes = 4 * 1024 * 1024 }, new OperationContext());
        }

        public async Task Delete(string container, string id)
        {
            await this.Init(container);
            await _container.GetBlockBlobReference(id).DeleteIfExistsAsync();
        }
    }
}
