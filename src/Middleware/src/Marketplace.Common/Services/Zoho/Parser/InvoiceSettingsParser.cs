using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of InvoiceSettingsApi.
    /// </summary>
    class InvoiceSettingsParser
    {
        
        internal static ZohoInvoiceSettings getInvoiceSettings(HttpResponseMessage response)
        {
            var invoiceSettings = new ZohoInvoiceSettings();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("invoice_settings"))
            {
                invoiceSettings = JsonConvert.DeserializeObject<ZohoInvoiceSettings>(jsonObj["invoice_settings"].ToString());
            }
            return invoiceSettings;
        }
    }
}
