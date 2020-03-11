using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Marketplace.Helpers.Exceptions.Models
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
}
