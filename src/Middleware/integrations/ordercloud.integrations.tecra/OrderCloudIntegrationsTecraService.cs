using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using ordercloud.integrations.library;
using ordercloud.integrations.tecra.Models;
using ordercloud.integrations.tecra.Storage;
using OrderCloud.SDK;
using Polly.Retry;
using Polly;
using Flurl.Http.Configuration;

namespace ordercloud.integrations.tecra
{
    public interface IOrderCloudIntegrationsTecraService
    {
        Task<TecraToken> GetToken();
        Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token);
        Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id);
        Task<string> GetTecraFrame(string token, string id);
        Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder);
        Task<string> GetTecraProofByStoreID(string token, string id);
        Task<string> GetTecraPDFByStoreID(string token, string id);

    }
    public class OrderCloudTecraConfig
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ChiliUrl { get; set; }
        public string Environment { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string WorkspaceID { get; set; }
        public string SettingsID { get; set; }
        public string StoreID { get; set; }
        public string BlobStorageHostUrl { get; set; }
        public string BlobStorageConnectionString { get; set; }
    }

    public class OrderCloudIntegrationsTecraService : IOrderCloudIntegrationsTecraService
    {
        private readonly IFlurlClient _flurl;
        private readonly IChiliBlobStorage _blob;
        public OrderCloudTecraConfig Config { get; }

        public OrderCloudIntegrationsTecraService(OrderCloudTecraConfig config, IChiliBlobStorage blob, IFlurlClientFactory flurlFactory)
        {
            Config = config;
            _blob = blob;
            _flurl = flurlFactory.Get($"{Config.BaseUrl}/");
        }
        private IFlurlRequest Token(string resource)
        {
            return _flurl.Request($"{resource}");
        }
        private IFlurlRequest Request(string resource, string token)
        {
            return _flurl.Request($"{resource}").WithOAuthBearerToken(token);
        }
        public async Task<TecraToken> GetToken() 
        {
            return await this.Token("auth/token")
                .PostStringAsync($"grant_type=client_credentials&client_id={Config.ClientId}&client_secret={Config.ClientSecret}")
                .ReceiveJson<TecraToken>();
        }

        public async Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token)
        {
            return await this.Request("api/chili/documents", token)
                .SetQueryParam("storeid", Config.StoreID)
                .GetJsonAsync<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder)
        {
            return await this.Request("api/chili/alldocuments", token).PostJsonAsync(new TecraDocumentRequest
            {
                chiliurl = Config.ChiliUrl,
                environment = Config.Environment,
                username = Config.Username,
                password = Config.Password,
                folder = folder
            }).ReceiveJson<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id)
        {
            return await this.Request($"api/chili/{id}/variabledefinitions", token)
                .SetQueryParam("storeid", Config.StoreID)
                .GetJsonAsync<TecraSpec[]>();
        }
        public async Task<string> GetTecraFrame(string token, string id)
        {
            return await this.Request($"api/v1/chili/loadtemplatebystoreid", token).SetQueryParams(new TecraFrameParams
            {
                docid = id,
                storeid = Config.StoreID,
                wsid = Config.WorkspaceID,
                folder = "root",
                vpid = ""
            }).GetJsonAsync<string>();
        }

        private AsyncRetryPolicy Retry()
        {
            // retries three times, waits five seconds in-between failures
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5),
                });
        }
        private async Task<string> DownloadProof(string token, string id) 
        {
            var file = await this.Request("api/v1/chili/getproofimagebystoreid", token)
                .SetQueryParams(new TecraProofParams { docid = id, storeid = Config.StoreID, page = 1 })
                .GetBytesAsync();
            return await _blob.UploadAsset($"{id}.png", file, "image/png");
        }
        
        private async Task<string> DownloadPDF(string token, string id) 
        {
            var file = await this.Request("api/v1/chili/getproofimagebystoreid", token)
                .SetQueryParams(new TecraPDFParams { docid = id, storeid = Config.StoreID, settingsid = Config.SettingsID })
                .GetBytesAsync();
            return await _blob.UploadAsset($"{id}.png", file, "application/pdf");
        }

        public async Task<string> GetTecraProofByStoreID(string token, string id)
        {
            return await Retry().ExecuteAsync(() => DownloadProof(token, id));
            
        }
        public async Task<string> GetTecraPDFByStoreID(string token, string id)
        {
            return await Retry().ExecuteAsync(() => DownloadPDF(token, id));

        }
    }
}
