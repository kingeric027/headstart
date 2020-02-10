using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of CustomerPaymentsApi.
    /// </summary>
    class CustomerPaymentParser
    {
        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoCustomerPaymentList getPaymentList(HttpResponseMessage responce)
        {
            var customerPaymentlist = new ZohoCustomerPaymentList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("customerpayments"))
            {
                var paymentsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["customerpayments"].ToString());
                foreach(var paymentObj in paymentsArray)
                {
                    var payment = new ZohoCustomerPayment();
                    payment = JsonConvert.DeserializeObject<ZohoCustomerPayment>(paymentObj.ToString());
                    customerPaymentlist.Add(payment);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                customerPaymentlist.ZohoPageContext = pageContext;
            }
            return customerPaymentlist;
        }

        internal static ZohoCustomerPayment getPayment(HttpResponseMessage responce)
        {
            var payment = new ZohoCustomerPayment();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("payment"))
            {
                payment = JsonConvert.DeserializeObject<ZohoCustomerPayment>(jsonObj["payment"].ToString());
            }
            return payment;
        }
    }
}
