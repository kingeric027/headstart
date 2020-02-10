using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Class SalesorderParser.
    /// </summary>
    class SalesorderParser
    {

        /// <summary>
        /// Gets the salesorder list.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>ZohoSalesorderList.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static ZohoSalesorderList getSalesorderList(HttpResponseMessage responce)
        {
            var salesorderList = new ZohoSalesorderList();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if(jsonObject.ContainsKey("salesorders"))
            {
                var salesordersArray = JsonConvert.DeserializeObject<List<object>>(jsonObject["salesorders"].ToString());
                foreach(var salesorderObj in salesordersArray)
                {
                    var salesorder = new ZohoSalesorder();
                    salesorder = JsonConvert.DeserializeObject<ZohoSalesorder>(salesorderObj.ToString());
                    salesorderList.Add(salesorder);
                }
            }
            if (jsonObject.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObject["page_context"].ToString());
                salesorderList.ZohoPageContext = pageContext;
            }
            return salesorderList;
        }

        /// <summary>
        /// Gets the salesorder.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>ZohoSalesorder.</returns>
        internal static ZohoSalesorder getSalesorder(HttpResponseMessage responce)
        {
            var salesorder = new ZohoSalesorder();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("salesorder"))
            {
                salesorder = JsonConvert.DeserializeObject<ZohoSalesorder>(jsonObject["salesorder"].ToString());
            }
            return salesorder;
        }
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Gets the email.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>ZohoEmail.</returns>
        internal static ZohoEmail getEmailContent(HttpResponseMessage responce)
        {
            var emailContent = new ZohoEmail();
            var jsonobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if(jsonobj.ContainsKey("data"))
            {
                emailContent = JsonConvert.DeserializeObject<ZohoEmail>(jsonobj["data"].ToString());
            }
            return emailContent;
        }

        /// <summary>
        /// Gets the template list.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>ZohoTemplateList.</returns>
        internal static ZohoTemplateList getTemplateList(HttpResponseMessage responce)
        {
            var templateList = new ZohoTemplateList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("templates"))
            {
                var templatesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["templates"].ToString());
                foreach (var templateObj in templatesArray)
                {
                    var template = new ZohoTemplate();
                    template = JsonConvert.DeserializeObject<ZohoTemplate>(templateObj.ToString());
                    templateList.Add(template);
                }
            }
            return templateList;
        }

        /// <summary>
        /// Gets the ZohoAddress.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>ZohoAddress.</returns>
        internal static ZohoAddress getAddress(HttpResponseMessage response)
        {
            var address = new ZohoAddress();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if(jsonObj.ContainsKey("billing_address"))
            {
                address = JsonConvert.DeserializeObject<ZohoAddress>(jsonObj["BillingZohoAddress"].ToString());
            }
            if (jsonObj.ContainsKey("shipping_address"))
            {
                address = JsonConvert.DeserializeObject<ZohoAddress>(jsonObj["ShippingZohoAddress"].ToString());
            }
            return address;
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
