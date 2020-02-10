using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of RecurringExpensesApi.
    /// </summary>
    class RecurringExpenseParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoRecurringExpenseList getRecurringExpenseList(HttpResponseMessage responce)
        {
            var recExpList = new ZohoRecurringExpenseList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("recurring_expenses"))
            {
                var recExpArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["recurring_expenses"].ToString());
                foreach(var recExpObj in recExpArray)
                {
                    var recExp = new ZohoRecurringExpense();
                    recExp = JsonConvert.DeserializeObject<ZohoRecurringExpense>(recExpObj.ToString());
                    recExpList.Add(recExp);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                recExpList.ZohoPageContext = pageContext;
            }
            return recExpList;
        }

        internal static ZohoRecurringExpense getRecurringExpense(HttpResponseMessage responce)
        {
            var recExp = new ZohoRecurringExpense();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("recurring_expense"))
            {
                recExp = JsonConvert.DeserializeObject<ZohoRecurringExpense>(jsonObj["recurring_expense"].ToString());
            }
            return recExp;
        }

        internal static ZohoExpenseList getExpenseHistory(HttpResponseMessage responce)
        {
            var expenseList = new ZohoExpenseList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("expensehistory"))
            {
                var expenseArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["expensehistory"].ToString());
                foreach(var expObj in expenseArray)
                {
                    var expense = new ZohoExpense();
                    expense = JsonConvert.DeserializeObject<ZohoExpense>(expObj.ToString());
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
    }
}
