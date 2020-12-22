using System;
using System.Collections.Generic;
using System.Text;
using OrderCloud.SDK;

namespace ordercloud.integrations.cardconnect
{
    public class CreditCardIntegrationException : Exception
    {
        public ApiError ApiError { get; }
        public CardConnectResponse Response { get; }

        public CreditCardIntegrationException(ApiError error, CardConnectResponse response)
        {
            ApiError = error;
            Response = response;
        }

        public CreditCardIntegrationException(string errorCode, string message, CardConnectResponse data)
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
