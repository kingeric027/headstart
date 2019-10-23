using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using OrderCloud.SDK;

namespace Marketplace.Common.Exceptions
{
    public class ApiValidationError
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public object Errors { get; set; }

        public ApiValidationError(ModelStateDictionary dict)
        {
            this.ErrorCode = "400";
            this.Errors = dict.Keys.SelectMany(key => dict[key].Errors.Select(x => new ApiError { ErrorCode = key, Message = x.ErrorMessage }));
            this.Message = "Validation Failed";
        }
    }

    public class ApiErrorException : Exception
    {
        public ApiError ApiError { get; }

        public ApiErrorException(ApiError error)
        {
            ApiError = error;
        }

        public ApiErrorException(ErrorCode errorCode, object data)
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
    }

    /// <summary>
    /// Base class for errors that would throw something in the HTTP 400 range in the API.
    /// </summary>
    public class UserErrorException : ApiErrorException
    {
        public UserErrorException(string message) : base("InvalidRequest", 400, message, null) { }
    }

    public class ErrorCode
    {
        public ErrorCode(string code, int httpStatus, string defaultMessage)
        {
            Code = code;
            HttpStatus = httpStatus;
            DefaultMessage = defaultMessage;
        }

        public string Code { get; set; }
        public int HttpStatus { get; set; }
        public string DefaultMessage { get; set; }
    }

    public class ErrorCode<TData> : ErrorCode
    {
        public ErrorCode(string code, int httpStatus, string defaultMessage) : base(code, httpStatus, defaultMessage) { }
    }

    public static class ErrorCodes
    {
        public static IDictionary<string, ErrorCode> All { get; } = new Dictionary<string, ErrorCode>
        {
            { "NotFound", new ErrorCode("Not Found", 404, "Resource requested was not found") },
            { "Required", new ErrorCode("Required", 400, "Field is required") },
            { "WriteFailure", new ErrorCode("Write Failure", 400, "Failed to create record") },
            { "UnrecognizedType", new ErrorCode("UnrecognizedType", 400, "Unrecognized type") },
            { "List.InvalidProperty", new ErrorCode("Invalid property", 400, "Invalid property") },
            { "List.InvalidSortProperty", new ErrorCode("Invalid sort property", 400, "Invalid sort property") },
            { "List.InvalidSearchProperty", new ErrorCode("Invalid search property", 400, "Invalid search property") },
            { "List.InvalidType", new ErrorCode("Invalid type", 400, "Invalid type") }

        };

        public static class Auth
        {
            /// <summary>User does not have role(s) required to perform this action.</summary>
            public static readonly ErrorCode<InsufficientRolesError> InsufficientRoles = All["Auth.InsufficientRoles"] as ErrorCode<InsufficientRolesError>;
        }


        public static class List
        {
            /// <summary>Property does not exist.</summary>
            public static readonly ErrorCode<InvalidPropertyError> InvalidProperty = All["List.InvalidProperty"] as ErrorCode<InvalidPropertyError>;
            /// <summary>Property is not sortable.</summary>
            public static readonly ErrorCode<InvalidPropertyError> InvalidSortProperty = All["List.InvalidSortProperty"] as ErrorCode<InvalidPropertyError>;
            /// <summary>Property is not searchable.</summary>
            public static readonly ErrorCode<InvalidPropertyError> InvalidSearchProperty = All["List.InvalidSearchProperty"] as ErrorCode<InvalidPropertyError>;
            /// <summary>Value passed is not valid for property type.</summary>
            public static readonly ErrorCode<InvalidPropertyError> InvalidType = All["List.InvalidType"] as ErrorCode<InvalidPropertyError>;
        }
    }

    public class ApiError
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class InvalidPropertyError
    {
        public InvalidPropertyError(Type type, string name)
        {
            Property = $"{type.Name}.{name}";
        }
        public string Property { get; set; }
    }

    public class InsufficientRolesError
    {
        public IList<ApiRole> RequiredRoles { get; set; }
        public IList<ApiRole> AssignedRoles { get; set; }
    }
}
