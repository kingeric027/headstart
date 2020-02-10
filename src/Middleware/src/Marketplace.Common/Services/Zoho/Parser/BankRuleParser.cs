using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of BankRulesApi.
    /// </summary>
    class BankRuleParser
    {
        internal static ZohoRuleList getRuleList(HttpResponseMessage responce)
        {
            var ruleList = new ZohoRuleList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if(jsonObj.ContainsKey("rules"))
            {
                var rulesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["rules"].ToString());
                foreach(var ruleObj in rulesArray)
                {
                    var rule = new ZohoRule();
                    rule = JsonConvert.DeserializeObject<ZohoRule>(ruleObj.ToString());
                    ruleList.Add(rule);
                }
            }
            return ruleList;
        }

        internal static ZohoRule getRule(HttpResponseMessage responce)
        {
            var rule = new ZohoRule();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("rule"))
            {
                rule = JsonConvert.DeserializeObject<ZohoRule>(jsonObj["rule"].ToString());
            }
            return rule;
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
