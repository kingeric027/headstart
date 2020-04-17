using System;
using System.Net;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Exceptions.Models;
using Marketplace.Helpers.Models;

namespace Marketplace.Helpers.Exceptions
{
    public class ApiErrorException : Exception
    {
        public ApiError ApiError { get; }

        public ApiErrorException(ApiError error)
        {
            ApiError = error;
        }

        public ApiErrorException(IErrorCode errorCode, object data)
        {
            ApiError = new ApiError
            {
                ErrorCode = errorCode.Code,
                StatusCode = (HttpStatusCode)errorCode.HttpStatus,
                Message = errorCode.DefaultMessage,
                Data = data
            };
        }

        protected ApiErrorException(string errorCode, int status, string message, object data)
        {
            ApiError = new ApiError
            {
                ErrorCode = errorCode,
                StatusCode = (HttpStatusCode)status,
                Message = message,
                Data = data
            };
        }

        public class NotFoundException : ApiErrorException
        {
            public NotFoundException(string thingType, string interopID) : base("NotFound", 404, "Object not found.", new { ObjectType = thingType, ObjectID = interopID }) { }
        }

		public class DuplicateIdException : ApiErrorException
		{
			public DuplicateIdException() : base("IdExists", 409, "Object already exists.", null) { }
		}

		public class UserErrorException : ApiErrorException
        {
            public UserErrorException(string message) : base("InvalidRequest", 400, message, null) { }
        }
    }
}
