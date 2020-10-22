using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Flurl.Http;
using ordercloud.integrations.library;
using ordercloud.integrations.tecra.Models;
using OrderCloud.SDK;

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
    }



    public class OrderCloudIntegrationsTecraService : IOrderCloudIntegrationsTecraService
    {
        private readonly IFlurlClient _flurl;
        public OrderCloudTecraConfig Config { get; }

        public OrderCloudIntegrationsTecraService() : this(new OrderCloudTecraConfig())
        {
        }
        public OrderCloudIntegrationsTecraService(OrderCloudTecraConfig config)
        {
            Config = config;
            _flurl = new FlurlClient
            {
                BaseUrl = $"{Config.BaseUrl}/"
            };
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
            var request = "grant_type=client_credentials&client_id=" + Config.ClientId + "&client_secret=" + Config.ClientSecret;
            return await this.Token("auth/token").PostStringAsync(request).ReceiveJson<TecraToken>();
        }

        public async Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token)
        {
            return await this.Request("api/chili/documents", token).SetQueryParam("storeid", Config.StoreID).GetJsonAsync<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder)
        {
            TecraDocumentRequest request = new TecraDocumentRequest();
            request.chiliurl = Config.ChiliUrl;
            request.environment = Config.Environment;
            request.username = Config.Username;
            request.password = Config.Password;
            request.folder = folder;
            return await this.Request("api/chili/alldocuments", token).PostJsonAsync(request).ReceiveJson<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id)
        {
            TecraDocumentRequest request = new TecraDocumentRequest();
            return await this.Request($"api/chili/{id}/variabledefinitions", token).SetQueryParam("storeid", Config.StoreID).GetJsonAsync<TecraSpec[]>();
        }
        public async Task<string> GetTecraFrame(string token, string id)
        {
            //TODO - Make wsid and folder dynamic
            TecraFrameParams tparams = new TecraFrameParams();
            tparams.docid = id;
            tparams.storeid= Config.StoreID;
            tparams.wsid = Config.WorkspaceID;
            tparams.wsid = "";
            tparams.folder = "root";
            tparams.vpid = "";

            return await this.Request($"api/v1/chili/loadtemplatebystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
        }
        public async Task<string> GetTecraProofByStoreID(string token, string id)
        {
            TecraProofParams tparams = new TecraProofParams();
            tparams.docid = id;
            tparams.storeid = Config.StoreID;
            tparams.page = 1;

            return await this.Request("api/v1/chili/getproofimagebystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
        }
        public async Task<string> GetTecraPDFByStoreID(string token, string id)
        {
            //TODO - Make wsid and folder dynamic
            TecraPDFParams tparams = new TecraPDFParams();
            tparams.docid = id;
            tparams.storeid = Config.StoreID;
            tparams.settingsid = Config.SettingsID;

            return await this.Request("api/v1/chili/generatepdfbystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
        }
    }
}
