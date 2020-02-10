using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.Zoho.Models
{
    public class ZohoToken
    {
        public string code { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
