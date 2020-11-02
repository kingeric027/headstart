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

        private IFlurlRequest Request(string resource)
        {
            return _flurl.Request($"{resource}").WithHeader("Authorization", $"Basic {Config.Authorization}");
        }

        public async Task<CardConnectAccountResponse> Tokenize(CardConnectAccountRequest request)
        {
            return await this.Request("cardsecure/api/v1/ccn/tokenize").PostJsonAsync(request).ReceiveJson<CardConnectAccountResponse>();
        }

        public async Task<CardConnectAuthorizationResponse> AuthWithoutCapture(CardConnectAuthorizationRequest request)
        {
            var attempt = await this
                .Request("cardconnect/rest/auth")
                .PutJsonAsync(request)
                .ReceiveJson<CardConnectAuthorizationResponse>();

            // Each payment processor has a unique set of response codes. Generally, a processor response code(respcode) beginning with "00" or "000" is a successful authorization request; any other code is a decline.  
            // https://developer.cardconnect.com/assets/developer/assets/authResp_2-11-19.txt
            if (attempt.IsExpired())
            {
                throw new CreditCardIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"Card has expired",
                    ErrorCode = attempt.respcode
                }, attempt);
            }
            else if(attempt.IsDeclined())
            {
                throw new CreditCardIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"Card was declined",
                    ErrorCode = attempt.respcode
                }, attempt);
            }
            else if (!attempt.PassedAVSCheck())
            {
                throw new CreditCardIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"Billing address on credit card incorrect",
                    ErrorCode = attempt.respcode
                }, attempt);
            }
            else if (!attempt.PassedCvvCheck(request))
            {
                throw new CreditCardIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"CVV Validation Failure",
                    ErrorCode = attempt.respcode
                }, attempt);
            }
            else if (!attempt.WasSuccessful())
            {
                throw new CreditCardIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"{attempt.respstat.ToResponseStatus()} : {attempt.resptext}",
                    ErrorCode = attempt.respcode
                }, attempt);
            }
            return attempt;

        }

        
    }
}
