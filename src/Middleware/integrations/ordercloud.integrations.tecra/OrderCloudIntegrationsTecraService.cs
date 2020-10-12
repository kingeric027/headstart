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
        Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token, string storeid);
        Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id, string folder);
        Task<string> GetTecraFrame(string token, string id, string storeid);
        Task<IEnumerable<TecraDocument>> TecraDocumentsByFolder(string token, string folder);
        Task<string> GetTecraProofByStoreID(string token, string id, string storeid);
        Task<string> GetTecraPDFByStoreID(string token, string id, string storeid);

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

        public async Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token, string storeid)
        {
            return await this.Request("api/chili/documents", token).SetQueryParam("storeid", storeid).GetJsonAsync<TecraDocument[]>();
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
        public async Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id, string folder)
        {
            TecraDocumentRequest request = new TecraDocumentRequest();
            return await this.Request($"api/chili/{id}/variabledefinitions", token).SetQueryParam("storeid", folder).GetJsonAsync<TecraSpec[]>();
        }
        public async Task<string> GetTecraFrame(string token, string id, string storeid)
        {
            //TODO - Make wsid and folder dynamic
            TecraFrameParams tparams = new TecraFrameParams();
            tparams.docid = id;
            tparams.storeid= storeid;
            tparams.wsid = "e3210f88-277a-4b4c-9758-6f45906f0958";
            tparams.wsid = "";
            tparams.folder = "root";
            tparams.vpid = "";

            return await this.Request($"api/v1/chili/loadtemplatebystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
        }
        public async Task<string> GetTecraProofByStoreID(string token, string id, string storeid)
        {
            TecraProofParams tparams = new TecraProofParams();
            tparams.docid = id;
            tparams.storeid = storeid;
            tparams.page = 1;

            return await this.Request("api/v1/chili/getproofimagebystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
        }
        public async Task<string> GetTecraPDFByStoreID(string token, string id, string storeid)
        {
            //TODO - Make wsid and folder dynamic
            TecraPDFParams tparams = new TecraPDFParams();
            tparams.docid = id;
            tparams.storeid = storeid;
            tparams.settingsid = "9549c0ca-36df-4aaa-a564-0e27d39ec7be";

            return await this.Request("api/v1/chili/generatepdfbystoreid", token).SetQueryParams(tparams).GetJsonAsync<string>();
        }
    }
}
