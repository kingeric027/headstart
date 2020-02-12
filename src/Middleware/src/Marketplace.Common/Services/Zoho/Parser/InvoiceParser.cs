using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of InvoiceApi.
    /// </summary>
    class InvoiceParser
    {
        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoInvoicesList getInvoiceList(HttpResponseMessage responce)
        {
            var invoiceList = new ZohoInvoicesList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("invoices"))
            {
                var invoicesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["invoices"].ToString());
                foreach(var invoiceObj in invoicesArray)
                {
                    var invoice = new ZohoInvoice();
                    invoice = JsonConvert.DeserializeObject<ZohoInvoice>(invoiceObj.ToString());
                    invoiceList.Add(invoice);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                invoiceList.ZohoPageContext = pageContext;
            }
            return invoiceList;
        }

        internal static ZohoInvoice getInvoice(HttpResponseMessage responce)
        {
            var invoice = new ZohoInvoice();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("invoice"))
            {
                invoice = JsonConvert.DeserializeObject<ZohoInvoice>(jsonObj["invoice"].ToString());
            }
            return invoice;
        }

        internal static ZohoPaymentList getPaymentsList(HttpResponseMessage responce)
        {
            var paymentList = new ZohoPaymentList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("payments"))
            {
                var paymentsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["payments"].ToString());
                foreach(var paymentObj in paymentsArray)
                {
                    var payment = new ZohoPayment();
                    payment = JsonConvert.DeserializeObject<ZohoPayment>(paymentObj.ToString());
                    paymentList.Add(payment);
                }
            }
            return paymentList;
        }

        internal static ZohoCreditNoteList getCredits(HttpResponseMessage responce)
        {
            var creditList = new ZohoCreditNoteList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("credits"))
            {
                var paymentsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["credits"].ToString());
                foreach (var paymentObj in paymentsArray)
                {
                    var credit = new ZohoCreditNote();
                    credit = JsonConvert.DeserializeObject<ZohoCreditNote>(paymentObj.ToString());
                    creditList.Add(credit);
                }
            }
            return creditList;
        }

        internal static ZohoUseCredits getUseCredits(HttpResponseMessage responce)
        {
            var useCredits = new ZohoUseCredits(); 
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("use_credits"))
            {
                useCredits = JsonConvert.DeserializeObject<ZohoUseCredits>(jsonObj["use_credits"].ToString());
            }
            return useCredits;
        }
    }
}
