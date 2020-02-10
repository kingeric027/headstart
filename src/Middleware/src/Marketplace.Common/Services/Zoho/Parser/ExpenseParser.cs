using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of ExpensesApi.
    /// </summary>
    class ExpenseParser
    {

        internal static ZohoExpenseList getExpenseList(HttpResponseMessage responce)
        {
            var expenseList = new ZohoExpenseList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("expenses"))
            {
                var expensesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["expenses"].ToString());
                foreach(var expenseObj in expensesArray)
                {
                    var expense = new ZohoExpense();
                    expense = JsonConvert.DeserializeObject<ZohoExpense>(expenseObj.ToString());
                    expenseList.Add(expense);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                expenseList.ZohoPageContext = pageContext;
            }
            return expenseList;
        }

        internal static ZohoExpense getExpense(HttpResponseMessage responce)
        {
            var expense = new ZohoExpense();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("expense"))
            {
                expense = JsonConvert.DeserializeObject<ZohoExpense>(jsonObj["expense"].ToString());
            }
            return expense;
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
