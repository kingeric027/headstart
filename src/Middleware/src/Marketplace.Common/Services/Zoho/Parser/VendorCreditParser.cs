using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Class VendorCreditParser.
    /// </summary>
    class VendorCreditParser
    {
        /// <summary>
        /// Gets the vendor creditlist.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>ZohoVendorCreditList.</returns>
        internal static ZohoVendorCreditList getVendorCreditlist(HttpResponseMessage response)
        {
            var vendorCreditList = new ZohoVendorCreditList();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("vendor_credits"))
            {
                var vendorCreditsArray = JsonConvert.DeserializeObject<List<object>>(jsonObject["vendor_credits"].ToString());
                foreach(var vendorCreditObj in vendorCreditsArray)
                {
                    var vendorCredit = new ZohoVendorCredit();
                    vendorCredit = JsonConvert.DeserializeObject<ZohoVendorCredit>(vendorCreditObj.ToString());
                    vendorCreditList.Add(vendorCredit);
                }
            }
            if (jsonObject.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObject["page_context"].ToString());
                vendorCreditList.ZohoPageContext = pageContext;
            }
            return vendorCreditList;
        }

        /// <summary>
        /// Gets the vendor credit.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>ZohoVendorCredit.</returns>
        internal static ZohoVendorCredit getVendorCredit(HttpResponseMessage response)
        {
            var vendorCredit = new ZohoVendorCredit();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if(jsonObject.ContainsKey("vendor_credit"))
            {
                vendorCredit = JsonConvert.DeserializeObject<ZohoVendorCredit>(jsonObject["vendor_credit"].ToString());
            }
            return vendorCredit;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>System.String.</returns>
        internal static string getMessage(HttpResponseMessage response)
        {
            string message = "";
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("message"))
                message = jsonObject["message"].ToString();
            return message;
        }

        /// <summary>
        /// Gets the bills credited.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>ZohoBillList.</returns>
        internal static ZohoBillList getBillsCredited(HttpResponseMessage response)
        {
            var billList = new ZohoBillList();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if(jsonObject.ContainsKey("bills"))
            {
                var billsArray = JsonConvert.DeserializeObject<List<object>>(jsonObject["bills"].ToString());
                foreach(var billObject in billsArray)
                {
                    var bill = new ZohoBill();
                    bill = JsonConvert.DeserializeObject<ZohoBill>(billObject.ToString());
                    billList.Add(bill);
                }
            }
            return billList;
        }

        /// <summary>
        /// Gets the refund list.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>ZohoVendorCreditRefundList.</returns>
        internal static ZohoVendorCreditRefundList getRefundList(HttpResponseMessage response)
        {
            var refundList = new ZohoVendorCreditRefundList();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("vendor_credit_refunds"))
            {
                var refundsArray = JsonConvert.DeserializeObject<List<object>>(jsonObject["vendor_credit_refunds"].ToString());
                foreach(var refundObj in refundsArray)
                {
                    var refund = new ZohoVendorCreditRefund();
                    refund = JsonConvert.DeserializeObject<ZohoVendorCreditRefund>(refundObj.ToString());
                    refundList.Add(refund);
                }
            }
            if (jsonObject.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObject["page_context"].ToString());
                refundList.ZohoPageContext = pageContext;
            }
            return refundList;
        }

        /// <summary>
        /// Gets the refund.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>ZohoVendorCreditRefund.</returns>
        internal static ZohoVendorCreditRefund getRefund(HttpResponseMessage response)
        {
            var refund = new ZohoVendorCreditRefund();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("vendor_credit_refund"))
            {
                refund = JsonConvert.DeserializeObject<ZohoVendorCreditRefund>(jsonObject["vendor_credit_refund"].ToString());
            }
            return refund;
        }

        /// <summary>
        /// Gets the comment list.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>ZohoCommentList.</returns>
        internal static ZohoCommentList getCommentList(HttpResponseMessage responce)
        {
            var commentList = new ZohoCommentList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("comments"))
            {
                var commentsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["comments"].ToString());
                foreach (var commentObj in commentsArray)
                {
                    var comment = new ZohoComment();
                    comment = JsonConvert.DeserializeObject<ZohoComment>(commentObj.ToString());
                    commentList.Add(comment);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                commentList.ZohoPageContext = pageContext;
            }
            return commentList;
        }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>ZohoComment.</returns>
        internal static ZohoComment getComment(HttpResponseMessage responce)
        {
            var comment = new ZohoComment();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("comment"))
            {
                comment = JsonConvert.DeserializeObject<ZohoComment>(jsonObj["comment"].ToString());
            }
            return comment;
        }
    }
}
