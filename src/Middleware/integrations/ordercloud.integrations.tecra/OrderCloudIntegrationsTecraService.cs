using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using ordercloud.integrations.tecra.Models;
using ordercloud.integrations.tecra.Storage;

namespace ordercloud.integrations.tecra
{
    public interface IOrderCloudIntegrationsTecraService
    {
        Task<TecraToken> GetToken();
        Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token);
        Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id);
        Task<string> GetTecraFrame(string token, string id);
        Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder);
        Task<string> DownloadProof(string token, string id);
        Task<string> DownloadPDF(string token, string id);

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
        private readonly OrderCloudTecraConfig _config;

        public OrderCloudIntegrationsTecraService(OrderCloudTecraConfig config, IChiliBlobStorage blob, IFlurlClientFactory flurlFactory)
        {
            _config = config;
            _blob = blob;
            _flurl = flurlFactory.Get($"{_config.BaseUrl}/");
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
                .PostStringAsync($"grant_type=client_credentials&client_id={_config.ClientId}&client_secret={_config.ClientSecret}")
                .ReceiveJson<TecraToken>();
        }

        public async Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token)
        {
            return await this.Request("api/chili/documents", token)
                .SetQueryParam("storeid", _config.StoreID)
                .GetJsonAsync<TecraDocument[]>();
        }

        public async Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder)
        {
            return await this.Request("api/chili/alldocuments", token).PostJsonAsync(new TecraDocumentRequest
            {
                chiliurl = _config.ChiliUrl,
                environment = _config.Environment,
                username = _config.Username,
                password = _config.Password,
                folder = folder
            }).ReceiveJson<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id)
        {
            return await this.Request($"api/chili/{id}/variabledefinitions", token)
                .SetQueryParam("storeid", _config.StoreID)
                .GetJsonAsync<TecraSpec[]>();
        }
        public async Task<string> GetTecraFrame(string token, string id)
        {
            return await this.Request($"api/v1/chili/loadtemplatebystoreid", token).SetQueryParams(new TecraFrameParams
            {
                docid = id,
                storeid = _config.StoreID,
                wsid = _config.WorkspaceID,
                folder = "root",
                vpid = ""
            }).GetJsonAsync<string>();
        }

        public async Task<string> DownloadProof(string token, string id)
        {
            var url = await this.Request("api/v1/chili/getproofimagebystoreid", token)
                .SetQueryParams(new TecraProofParams { docid = id, storeid = _config.StoreID, page = 1 })
                .GetJsonAsync<string>();
            var file = await url.GetBytesAsync();
            return await _blob.UploadAsset($"{id}.png", file, "image/png");
        }
        public async Task<string> DownloadPDF(string token, string id)
        {
            var url = await this.Request("api/v1/chili/generatepdfbystoreid", token)
                .SetQueryParams(new TecraPDFParams { docid = id, storeid = _config.StoreID, settingsid = _config.SettingsID }).GetJsonAsync<string>();
            var file = await url.GetBytesAsync();
            return await _blob.UploadAsset($"{id}.pdf", file, "application/pdf");
        }
    }
}
