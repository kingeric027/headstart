using System;
using System.Collections.Generic;
using System.Text;
using OrderCloud.SDK;

namespace ordercloud.integrations.cms
{
    public class DuplicateIdException : Exception
    {
        public ApiError ApiError { get; }

        public DuplicateIdException(ApiError error)
        {
            ApiError = error;
        }

        public DuplicateIdException(string errorCode, string message, object data)
        {
            ApiError = new ApiError
            {
                ErrorCode = errorCode,
                Message = message,
                Data = data
            };
        }
    }
}
