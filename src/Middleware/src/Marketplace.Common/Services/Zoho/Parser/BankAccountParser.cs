using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of BankAccountsApi.
    /// </summary>
    
     class BankAccountParser
    {
        internal static ZohoBankAccountList getBankAccountList(HttpResponseMessage responce)
        {
            var bankAccountList = new ZohoBankAccountList();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("bankaccounts"))
            {
                var bankAccountsArray = JsonConvert.DeserializeObject<List<object>>(jsonObject["bankaccounts"].ToString());
                foreach (var bankAccountObj in bankAccountsArray)
                {
                    var bankAccount = new ZohoBankAccount();
                    bankAccount = JsonConvert.DeserializeObject<ZohoBankAccount>(bankAccountObj.ToString());
                    bankAccountList.Add(bankAccount);
                }
            }
            if (jsonObject.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObject["page_context"].ToString());
                bankAccountList.ZohoPageContext = pageContext;
            }
            return bankAccountList;
        }

        internal static ZohoBankAccount getBankAccount(HttpResponseMessage responce)
        {
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            var bankAccount = new ZohoBankAccount();
            if (jsonObj.ContainsKey("bankaccount"))
            {
                bankAccount = JsonConvert.DeserializeObject<ZohoBankAccount>(jsonObj["bankaccount"].ToString());
            }
            return bankAccount;
        }

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
            {
                message = jsonObj["message"].ToString();
            }
            return message;
        }

        internal static ZohoStatement getStatement(HttpResponseMessage responce)
        {
            var statement = new ZohoStatement();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string,object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("statement"))
            {
                statement = JsonConvert.DeserializeObject<ZohoStatement>(jsonObj["statement"].ToString());
            }
            return statement;
        }
    }
}
