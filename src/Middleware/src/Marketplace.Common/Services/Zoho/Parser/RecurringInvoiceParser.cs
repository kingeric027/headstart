using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of RecurringInvoicesApi.
    /// </summary>
    class RecurringInvoiceParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoRecurringInvoiceList getRecurringInvoiceList(HttpResponseMessage responce)
        {
            var recInvoiceList = new ZohoRecurringInvoiceList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("recurring_invoices"))
            {
                var recInvoiceArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["recurring_invoices"].ToString());
                foreach (var recInvoiceObj in recInvoiceArray)
                {
                    var recInvoice = new ZohoRecurringInvoice();
                    recInvoice = JsonConvert.DeserializeObject<ZohoRecurringInvoice>(recInvoiceObj.ToString());
                    recInvoiceList.Add(recInvoice);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                recInvoiceList.ZohoPageContext = pageContext;
            }
            return recInvoiceList;
        }

        internal static ZohoRecurringInvoice getRecurringInvoice(HttpResponseMessage responce)
        {
            var recInvoice = new ZohoRecurringInvoice();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("recurring_invoice"))
            {
                recInvoice = JsonConvert.DeserializeObject<ZohoRecurringInvoice>(jsonObj["recurring_invoice"].ToString());
            }
            return recInvoice;
        }
    }
}
