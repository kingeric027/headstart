using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Class PurchaseorderParser.
    /// </summary>
    class PurchaseorderParser
    {

        /// <summary>
        /// Gets the purchaseorder list.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>ZohoPurchaseorderList.</returns>
        internal static ZohoPurchaseorderList getPurchaseorderList(HttpResponseMessage response)
        {
            var purchaseorderList = new ZohoPurchaseorderList();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("purchaseorders"))
            {
                var purchaseordersArray = JsonConvert.DeserializeObject<List<object>>(jsonObject["purchaseorders"].ToString());
                foreach (var purchaseorderObj in purchaseordersArray)
                {
                    var purchaseorder = new ZohoPurchaseOrder();
                    purchaseorder = JsonConvert.DeserializeObject<ZohoPurchaseOrder>(purchaseorderObj.ToString());
                    purchaseorderList.Add(purchaseorder);
                }
            }
            if (jsonObject.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObject["page_context"].ToString());
                purchaseorderList.ZohoPageContext = pageContext;
            }
            return purchaseorderList;
        }

        /// <summary>
        /// Gets the purchaseorder.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Purchaseorder.</returns>
        internal static ZohoPurchaseOrder getPurchaseorder(HttpResponseMessage response)
        {
            var purchaseorder = new ZohoPurchaseOrder();
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObject.ContainsKey("purchaseorder"))
            {
                purchaseorder = JsonConvert.DeserializeObject<ZohoPurchaseOrder>(jsonObject["purchaseorder"].ToString());
            }
            return purchaseorder;
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
        /// Gets the content of the email.
        /// </summary>
        /// <param name="responce">The responce.</param>
        /// <returns>ZohoEmail.</returns>
        internal static ZohoEmail getEmailContent(HttpResponseMessage responce)
        {
            var emailContent = new ZohoEmail();
            var jsonobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonobj.ContainsKey("data"))
            {
                emailContent = JsonConvert.DeserializeObject<ZohoEmail>(jsonobj["data"].ToString());
            }
            return emailContent;
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
            if (jsonObj.ContainsKey("billing_address"))
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
