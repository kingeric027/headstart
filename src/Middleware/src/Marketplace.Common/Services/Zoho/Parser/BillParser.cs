using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of BillsApi.
    /// </summary>
    class BillParser
    {
        internal static ZohoBillList getBillList(HttpResponseMessage responce)
        {
            var billList = new ZohoBillList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("bills"))
            {
                var billsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["bills"].ToString());
                foreach(var billObj in billsArray)
                {
                    var bill = new ZohoBill();
                    bill = JsonConvert.DeserializeObject<ZohoBill>(billObj.ToString());
                    billList.Add(bill);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                billList.ZohoPageContext = pageContext;
            }
            return billList;
        }

        internal static ZohoBill getBill(HttpResponseMessage responce)
        {
            var bill=new ZohoBill();
            var json = responce.Content.ReadAsStringAsync().Result;
            var jsonObj = JObject.Parse(json).ToObject<Dictionary<string, object>>();
            //var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json.ToString());
            if(jsonObj.ContainsKey("bill"))
            {
                bill = JsonConvert.DeserializeObject<ZohoBill>(jsonObj["bill"].ToString());
            }
            return bill;
        }

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var json = responce.Content.ReadAsStringAsync().Result;
            var jsonObj = JObject.Parse(json).ToObject<Dictionary<string, object>>();
            //var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoPaymentList getPaymentList(HttpResponseMessage responce)
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

        internal static ZohoCommentList getCommentsList(HttpResponseMessage responce)
        {
            var commentList = new ZohoCommentList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("comments"))
            {
                var commentsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["comments"].ToString());
                foreach(var commentObj in commentsArray)
                {
                    var comment = new ZohoComment();
                    comment = JsonConvert.DeserializeObject<ZohoComment>(commentObj.ToString());
                    commentList.Add(comment);
                }
            }

            return commentList;
        }
    }
}
