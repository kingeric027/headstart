using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.Zoho
{
    public class ZohoClientConfig
    {
        public string AuthToken { get; set; }
        public string OrganizationID { get; set; }
        public string ApiUrl { get; set; } = "https://books.zoho.com/api/v3";
    }
}
