using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of EstimateApi.
    /// </summary>
    class EstimateParser
    {
        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoEstimateList getEstimateList(HttpResponseMessage responce)
        {
            var estimateList = new ZohoEstimateList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("estimates"))
            {
                var estimatesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["estimates"].ToString());
                foreach(var estimateObj in estimatesArray)
                {
                    var estimate = new ZohoEstimate();
                    estimate = JsonConvert.DeserializeObject<ZohoEstimate>(estimateObj.ToString());
                    estimateList.Add(estimate);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                estimateList.ZohoPageContext = pageContext;
            }
            return estimateList;
        }

        internal static ZohoEstimate getEstimate(HttpResponseMessage responce)
        {
            var estimate = new ZohoEstimate();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("estimate"))
            {
                estimate = JsonConvert.DeserializeObject<ZohoEstimate>(jsonObj["estimate"].ToString());
            }
            return estimate;
        }
    }
}
