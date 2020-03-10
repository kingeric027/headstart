using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Marketplace.Helpers.Exceptions.Models
{
    public class ApiError
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
