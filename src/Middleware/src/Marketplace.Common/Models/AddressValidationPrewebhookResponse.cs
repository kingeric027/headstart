using Marketplace.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    public class AddressValidationPrewebhookResponse
    {
        public bool proceed { get; set; }

        // body is an abitrary addition to the prewebhook response which displays on the front end in a toastr
        public string body { get; set; }
    }
}


