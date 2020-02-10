using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of BankTransactionsApi.
    /// </summary>
    class BankTransactionParser
    {
        internal static ZohoTransactionList getTransactionList(HttpResponseMessage responce)
        {
            var transactionList = new ZohoTransactionList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("banktransactions"))
            {
                var transactionArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["banktransactions"].ToString());
                foreach(var transactionObj in transactionArray)
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

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoTransaction getTransaction(HttpResponseMessage responce)
        {
            var transaction = new ZohoTransaction();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("banktransaction"))
            {
                transaction = JsonConvert.DeserializeObject<ZohoTransaction>(jsonObj["banktransaction"].ToString());
            }
            return transaction;
        }

        internal static ZohoMatchingTransactions getMatchingTransactions(HttpResponseMessage responce)
        {
            var matchingTransactions = new ZohoMatchingTransactions();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("matching_transactions"))
            {
                var transArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["matching_transactions"].ToString());
                foreach(var transactionObj in transArray)
                {
                    var transaction = new ZohoTransaction();
                    transaction = JsonConvert.DeserializeObject<ZohoTransaction>(transactionObj.ToString());
                    matchingTransactions.Add(transaction);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                matchingTransactions.ZohoPageContext = pageContext;
            }
            if (jsonObj.ContainsKey("instrumentation"))
            {
                var instrumentation = new ZohoInstrumentation();
                instrumentation = JsonConvert.DeserializeObject<ZohoInstrumentation>(jsonObj["instrumentation"].ToString());
                matchingTransactions.ZohoInstrumentation = instrumentation;
            }
            return matchingTransactions;
        }

        internal static ZohoTransaction getAssociatedTransaction(HttpResponseMessage responce)
        {
            var transaction = new ZohoTransaction();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("transaction"))
            {
                transaction = JsonConvert.DeserializeObject<ZohoTransaction>(jsonObj["transaction"].ToString());
            }
            return transaction;
        }
    }
}
