using System;
using System.Threading.Tasks;
using Flurl.Http;
using OrderCloud.SDK;

namespace ordercloud.integrations.cardconnect
{
    public interface IOrderCloudIntegrationsCardConnectService
    {
        Task<CardConnectAccountResponse> Tokenize(CardConnectAccountRequest request);
        Task<CardConnectAuthorizationResponse> AuthWithoutCapture(CardConnectAuthorizationRequest request);
        Task<CardConnectAuthorizationResponse> AuthWithCapture(CardConnectAuthorizationRequest request);
    }

    public class OrderCloudIntegrationsCardConnectConfig 
    {
        public string Site { get; set; }
        public string BaseUrl { get; set; }
        public string Authorization { get; set; }
        public string UsdMerchantID { get; set; }
        public string CadMerchantID { get; set; }
        public string EurMerchantID { get; set; }
    }

    public class OrderCloudIntegrationsCardConnectService : IOrderCloudIntegrationsCardConnectService
    {
        private readonly IFlurlClient _flurl;
        public OrderCloudIntegrationsCardConnectConfig Config { get; }

        public OrderCloudIntegrationsCardConnectService() : this(new OrderCloudIntegrationsCardConnectConfig())
        {
        }

        public OrderCloudIntegrationsCardConnectService(OrderCloudIntegrationsCardConnectConfig config)
        {
            Config = config;
            _flurl = new FlurlClient
            {
                BaseUrl = $"https://{Config.Site}.{Config.BaseUrl}/"
            };
        }

        private IFlurlRequest Request(string resource, string currency = null)
        {
            return _flurl.Request($"{resource}").WithHeader("Authorization", $"Basic {((currency == "USD") ? Config.Authorization : "c2VidmVuZG9yNnptQDQ5YmNMIXdEVDl3I1lOUA==")}");
        }

        public async Task<CardConnectAccountResponse> Tokenize(CardConnectAccountRequest request)
        {
            return await this.Request("cardsecure/api/v1/ccn/tokenize").PostJsonAsync(request).ReceiveJson<CardConnectAccountResponse>();
        }

        public async Task<CardConnectAuthorizationResponse> AuthWithoutCapture(CardConnectAuthorizationRequest request)
        {
            return await PostAuthorizationAsync(request);
        }

        public async Task<CardConnectAuthorizationResponse> AuthWithCapture(CardConnectAuthorizationRequest request)
        {
            request.capture = "Y";
            return await PostAuthorizationAsync(request);
        }

        private async Task<CardConnectAuthorizationResponse> PostAuthorizationAsync(CardConnectAuthorizationRequest request)
        {
            var attempt = await this
                .Request("cardconnect/rest/auth", request.currency)
                .PutJsonAsync(request)
                .ReceiveJson<CardConnectAuthorizationResponse>();

            if (attempt.WasSuccessful())
            {
                return attempt;
            }
            throw new CreditCardIntegrationException(new ApiError()
            {
                Data = attempt,
                Message = attempt.resptext, // response codes: https://developer.cardconnect.com/assets/developer/assets/authResp_2-11-19.txt
                ErrorCode = attempt.respcode
            }, attempt);
        }
    }
}
