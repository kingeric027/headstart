using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of ChartofaccountsApi.
    /// </summary>
    class ChartofaccountParser
    {
        internal static ZohoChartOfAccountList getChartOfAccountList(HttpResponseMessage responce)
        {
            var chartOfAccountList = new ZohoChartOfAccountList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("chartofaccounts"))
            {
                var chartOfAccountsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["chartofaccounts"].ToString());
                foreach(var chartOfAccountObj in chartOfAccountsArray)
                {
                    var chartOfAccount = new ZohoChartOfAccount();
                    chartOfAccount = JsonConvert.DeserializeObject<ZohoChartOfAccount>(chartOfAccountObj.ToString());
                    chartOfAccountList.Add(chartOfAccount);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                chartOfAccountList.ZohoPageContext = pageContext;
            }
            return chartOfAccountList;
        }

        internal static ZohoChartOfAccount getChartOfAccount(HttpResponseMessage responce)
        {
            var chartOfAccount = new ZohoChartOfAccount();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("chart_of_account"))
            {
                chartOfAccount = JsonConvert.DeserializeObject<ZohoChartOfAccount>(jsonObj["chart_of_account"].ToString());
            }
            return chartOfAccount;
        }

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoTransactionList getTransactionList(HttpResponseMessage responce)
        {
            var transactionList = new ZohoTransactionList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("transactions"))
            {
                var transactionArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["transactions"].ToString());
                foreach (var transactionObj in transactionArray)
                {
                    var transaction = new ZohoTransaction();
                    transaction = JsonConvert.DeserializeObject<ZohoTransaction>(transactionObj.ToString());
                    transactionList.Add(transaction);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                transactionList.ZohoPageContext = pageContext;
            }
            return transactionList;
        }
    }
}
