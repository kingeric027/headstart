using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of currenciesApi.
    /// </summary>
    class CurrencyParser
    {
        
        internal static ZohoCurrencyList getCurrencyList(HttpResponseMessage response)
        {
            var currencylist = new ZohoCurrencyList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("currencies"))
            {
                var currencyArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["currencies"].ToString());
                foreach(var currencyObj in currencyArray)
                {
                    var currency = new ZohoCurrency();
                    currency = JsonConvert.DeserializeObject<ZohoCurrency>(currencyObj.ToString());
                    currencylist.Add(currency);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                currencylist.ZohoPageContext = pageContext;
            }
            return currencylist;
        }

        internal static ZohoCurrency getCurrency(HttpResponseMessage response)
        {
            var currency = new ZohoCurrency();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("currency"))
            {
                currency = JsonConvert.DeserializeObject<ZohoCurrency>(jsonObj["currency"].ToString());
            }
            return currency;
        }

        internal static ZohoExchangeRateList getExchangeRateList(HttpResponseMessage response)
        {
            var rateList = new ZohoExchangeRateList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("exchange_rates"))
            {
                var ratesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["exchange_rates"].ToString());
                foreach(var rateObj in ratesArray)
                {
                    var rate = new ZohoExchangeRate();
                    rate = JsonConvert.DeserializeObject<ZohoExchangeRate>(rateObj.ToString());
                    rateList.Add(rate);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                rateList.ZohoPageContext = pageContext;
            }
            return rateList;
        }

        internal static ZohoExchangeRate getExchangeRate(HttpResponseMessage response)
        {
            var rate = new ZohoExchangeRate();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("exchange_rate"))
            {
                rate = JsonConvert.DeserializeObject<ZohoExchangeRate>(jsonObj["exchange_rate"].ToString());
            }
            return rate;
        }
    }
}
