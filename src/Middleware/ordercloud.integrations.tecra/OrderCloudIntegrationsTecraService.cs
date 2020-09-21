using System.Collections.Generic;
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
        Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token, string folder);
        Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id);

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

        public async Task<IEnumerable<TecraDocument>> GetTecraDocuments(string token, string folder)
        {
            return await this.Request("api/chili/alldocuments", token).SetQueryParam("storeid", folder).GetJsonAsync<TecraDocument[]>();
        }
        public async Task<IEnumerable<TecraSpec>> GetTecraSpecs(string token, string id)
        {
            TecraDocumentRequest request = new TecraDocumentRequest();
            request.chiliurl = Config.ChiliUrl;
            request.environment = Config.Environment;
            request.username = Config.Username;
            request.password = Config.Password;
            request.folder = "";
            return await this.Request($"api/chili/{id}/variabledefinitions", token).PostJsonAsync(request).ReceiveJson<TecraSpec[]>();
        }
    }
}
