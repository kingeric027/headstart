using System.Threading.Tasks;
using Flurl.Http;
using ordercloud.integrations.library;
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
            if (!PassedAVSCheck(attempt))
            {
                throw new OrderCloudIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"Billing address on credit card incorrect",
                    ErrorCode = attempt.respcode
                });
            }
            else if (!PassedCvvCheck(attempt, request))
            {
                throw new OrderCloudIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"CVV Validation Failure",
                    ErrorCode = attempt.respcode
                });
            }
            else if (!WasSuccessful(attempt))
            {
                throw new OrderCloudIntegrationException(new ApiError()
                {
                    Data = attempt,
                    Message = $"{attempt.respstat.ToResponseStatus()} : {attempt.resptext}",
                    ErrorCode = attempt.respcode
                });
            }
            return attempt;

        }

        private static bool WasSuccessful(CardConnectAuthorizationResponse attempt)
        {
            return attempt.respstat == "A" && (attempt.respcode == "0" || attempt.respcode == "00" || attempt.respcode == "000");
        }

        private static bool PassedCvvCheck(CardConnectAuthorizationResponse attempt, CardConnectAuthorizationRequest request)
        {
            if (request.cvv2 == null && (attempt.cvvresp == "P" || attempt.cvvresp == null)) 
                return true;
            return (attempt.cvvresp != null &&
                    attempt.cvvresp != "N" && attempt.cvvresp != "P" && attempt.cvvresp != "U");
        }

        private static bool PassedAVSCheck(CardConnectAuthorizationResponse attempt)
        {
            return (attempt.avsresp != null &&
                    (attempt.avsresp != "N" && attempt.avsresp != "A" && attempt.avsresp != "Z"));
        }
    }
}
