using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Marketplace.Common.Mappers.CardConnect;
using Marketplace.Common.Services.CardConnect.Models;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Exceptions.Models;

namespace Marketplace.Common.Services.CardConnect
{
    public interface ICardConnectService
    {
        Task<AccountResponse> Tokenize(AccountRequest request);
        Task<AuthorizationResponse> AuthWithoutCapture(AuthorizationRequest request);
    }

    public class CardConnectService : ICardConnectService
    {
        private readonly AppSettings _settings;
        private readonly IFlurlClient _flurl;

        public CardConnectService(AppSettings settings)
        {
            _settings = settings;
            _flurl = new FlurlClient
            {
                BaseUrl = $"https://{_settings.CardConnectSettings.Site}.{settings.CardConnectSettings.BaseUrl}/"
            };
        }

        private IFlurlRequest Request(string resource)
        {
            return _flurl.Request($"{resource}").WithHeader("Authorization", $"Basic {_settings.CardConnectSettings.Authorization}");
        }

        public async Task<AccountResponse> Tokenize(AccountRequest request)
        {
            return await this.Request("cardsecure/api/v1/ccn/tokenize").PostJsonAsync(request).ReceiveJson<AccountResponse>();
        }

        public async Task<AuthorizationResponse> AuthWithoutCapture(AuthorizationRequest request)
        {
            var attempt = await this
                .Request("cardconnect/rest/auth")
                .PutJsonAsync(request)
                .ReceiveJson<AuthorizationResponse>();

            // Each payment processor has a unique set of response codes. Generally, a processor response code(respcode) beginning with "00" or "000" is a successful authorization request; any other code is a decline.  
            // https://developer.cardconnect.com/assets/developer/assets/authResp_2-11-19.txt
            if (!this.PassedAVSCheck(attempt))
            {
                throw new ApiErrorException(new ApiError()
                {
                    Data = attempt,
                    Message = $"AVS Validation Failure",
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = attempt.respcode
                });
            }
            else if (!this.PassedCvvCheck(attempt, request))
            {
                throw new ApiErrorException(new ApiError()
                {
                    Data = attempt,
                    Message = $"CVV Validation Failure",
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = attempt.respcode
                });
            }
            else if (!this.WasSuccessful(attempt))
            {
                throw new ApiErrorException(new ApiError()
                {
                    Data = attempt,
                    Message = $"{attempt.respstat.ToResponseStatus().ToString()} : {attempt.resptext}",
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = attempt.respcode
                });
            }
            return attempt;

        }

        private bool WasSuccessful(AuthorizationResponse attempt)
        {
            return attempt.respstat == "A" && (attempt.respcode == "0" || attempt.respcode == "00" || attempt.respcode == "000");
        }

        private bool PassedCvvCheck(AuthorizationResponse attempt, AuthorizationRequest request)
        {
            if (request.cvv2 == null && (attempt.cvvresp == "P" || attempt.cvvresp == null)) 
                return true;
            return (attempt.cvvresp != null &&
                    attempt.cvvresp != "N" && attempt.cvvresp != "P" && attempt.cvvresp != "U");
        }

        private bool PassedAVSCheck(AuthorizationResponse attempt)
        {
            return (attempt.avsresp != null &&
                    (attempt.avsresp != "N" && attempt.avsresp != "A" && attempt.avsresp != "Z"));
        }
    }
}
