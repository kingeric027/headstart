using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    public class PrewebhookResponseWithError : WebhookResponse
    {
        // in the future consider a more robust error response format
        // this could affect front end displays of errors from prewebhooks
        public string body { get; set; }
    }
}


