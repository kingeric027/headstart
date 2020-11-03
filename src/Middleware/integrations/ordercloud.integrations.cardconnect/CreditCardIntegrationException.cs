using System;
using System.Collections.Generic;
using System.Text;
using OrderCloud.SDK;

namespace ordercloud.integrations.cardconnect
{
    public class CreditCardIntegrationException : Exception
    {
        public ApiError ApiError { get; }
        public CardConnectAuthorizationResponse Response { get; }

        public CreditCardIntegrationException(ApiError error, CardConnectAuthorizationResponse response)
        {
            ApiError = error;
            Response = response;
        }

        public CreditCardIntegrationException(string errorCode, string message, CardConnectAuthorizationResponse data)
        {
            ApiError = new ApiError()
            {
                Data = data,
                ErrorCode = errorCode,
                Message = message
            };
            Response = data;
        }
    }
}
