using System;
using System.Collections.Generic;
using Marketplace.Helpers.Exceptions.Models;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Helpers.Exceptions
{
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
            { "List.InvalidType", new ErrorCode("Invalid type", 400, "Invalid type") },
            { "Blob.ConnectionString", new ErrorCode("InvalidConnectionString", 404, "Invalid Connection String")},
            { "Blob.Container", new ErrorCode("InvalidContainerString", 404, "Invalid Container")}
        };

        public static class Auth
        {
            /// <summary>User does not have role(s) required to perform this action.</summary>
            public static readonly ErrorCode<InsufficientRolesError> InsufficientRoles = All["Auth.InsufficientRoles"] as ErrorCode<InsufficientRolesError>;
        }

        public static class Blob
        {
            public static readonly ErrorCode<BlobConfigurationError> InvalidConnectionString = All["Blob.ConnectionString"] as ErrorCode<BlobConfigurationError>;
            public static readonly ErrorCode<BlobConfigurationError> InvalidContainer = All["Blob.Container"] as ErrorCode<BlobConfigurationError>;
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

    public class InsufficientRolesError
    {
        public IList<ApiRole> RequiredRoles { get; set; }
        public IList<ApiRole> AssignedRoles { get; set; }
    }
}
