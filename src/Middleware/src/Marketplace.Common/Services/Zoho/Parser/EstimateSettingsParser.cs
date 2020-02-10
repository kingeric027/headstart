using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of EstimateSettingsApi.
    /// </summary>
    class EstimateSettingsParser
    {
        
        internal static ZohoEstimateSettings getEstimateSettings(System.Net.Http.HttpResponseMessage response)
        {
            var estimateSettings = new ZohoEstimateSettings();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("estimate_settings"))
            {
                estimateSettings = JsonConvert.DeserializeObject<ZohoEstimateSettings>(jsonObj["estimate_settings"].ToString());
            }
            return estimateSettings;
        }
    }
}
