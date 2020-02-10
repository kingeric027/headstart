using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of BaseCurrencyAdjustmentApi.
    /// </summary>
    class BaseCurrencyAdjustmentParser
    {
        internal static ZohoBaseCurrencyAdjustmentsList getBaseCurrencyAdjustmentList(HttpResponseMessage responce)
        {
            var baseCurrencyAdjustmentList = new ZohoBaseCurrencyAdjustmentsList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("base_currency_adjustments"))
            {
                var baseCurrencyAdjArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["base_currency_adjustments"].ToString());
                foreach(var baseCurrencyAdjObj in baseCurrencyAdjArray)
                {
                    var baseCurrencyAdj = new ZohoBaseCurrencyAdjustment();
                    baseCurrencyAdj = JsonConvert.DeserializeObject<ZohoBaseCurrencyAdjustment>(baseCurrencyAdjObj.ToString());
                    baseCurrencyAdjustmentList.Add(baseCurrencyAdj);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                baseCurrencyAdjustmentList.ZohoPageContext = pageContext;
            }
            return baseCurrencyAdjustmentList;
        }

        internal static ZohoBaseCurrencyAdjustment getBaseCurrencyAdjustment(HttpResponseMessage responce)
        {
            var baseCurrencyAdjustment = new ZohoBaseCurrencyAdjustment();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if(jsonObj.ContainsKey("data"))
            {
                baseCurrencyAdjustment = JsonConvert.DeserializeObject<ZohoBaseCurrencyAdjustment>(jsonObj["data"].ToString());
            }
            return baseCurrencyAdjustment;
        }

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }
    }
}
