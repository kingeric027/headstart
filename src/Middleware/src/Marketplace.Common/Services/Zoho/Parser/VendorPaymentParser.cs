using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of VendorPaymentsApi.
    /// </summary>
    class VendorPaymentParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoVendorPaymentList getVendorPaymentList(HttpResponseMessage responce)
        {
            var vendorPaymentList = new ZohoVendorPaymentList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("vendorpayments"))
            {
                var paymentsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["vendorpayments"].ToString());
                foreach (var paymentObj in paymentsArray)
                {
                    var payment = new ZohoVendorPayment();
                    payment = JsonConvert.DeserializeObject<ZohoVendorPayment>(paymentObj.ToString());
                    vendorPaymentList.Add(payment);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                vendorPaymentList.ZohoPageContext = pageContext;
            }
            return vendorPaymentList;
        }

        internal static ZohoVendorPayment getVendorPayment(HttpResponseMessage responce)
        {
            var payment = new ZohoVendorPayment();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("vendorpayment"))
            {
                payment = JsonConvert.DeserializeObject<ZohoVendorPayment>(jsonObj["vendorpayment"].ToString());
            }
            return payment;
        }
    }
}
