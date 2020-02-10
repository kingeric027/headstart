using System;
using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Parser;
using Marketplace.Common.Services.Zoho.Util;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.API
{
    /// <summary>
    /// Class PurchaseordersApi.
    /// </summary>
    public class PurchaseordersApi:Api
    {
        /// <summary>
        /// The base ZohoAddress
        /// </summary>
        static string baseAddress = baseurl + "/purchaseorders";
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseordersApi" /> class.
        /// </summary>
        /// <param name="auth_token">The auth_token.</param>
        /// <param name="organization_Id">The organization_ identifier.</param>
        public PurchaseordersApi(string auth_token, string organization_Id)
            : base(auth_token, organization_Id)
        {

        }

        /// <summary>
        /// Gets the purchaseorders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ZohoPurchaseorderList.</returns>
        public ZohoPurchaseorderList GetPurchaseorders(Dictionary<object,object> parameters)
        {
            var url = baseAddress;
            var response = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return PurchaseorderParser.getPurchaseorderList(response);   
        }

        /// <summary>
        /// Gets the specified purchaseorder_id.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Purchaseorder.</returns>
        public ZohoPurchaseOrder Get(string purchaseorder_id,Dictionary<object,object> parameters)
        {
            var url = baseAddress + "/" + purchaseorder_id;
            var response = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return PurchaseorderParser.getPurchaseorder(response);
        }

        /// <summary>
        /// Creates the specified new_purchaseorder_info.
        /// </summary>
        /// <param name="newZohoPurchaseOrderInfo">The new_purchaseorder_info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Purchaseorder.</returns>
        public ZohoPurchaseOrder Create(ZohoPurchaseOrder newZohoPurchaseOrderInfo, Dictionary<object, object> parameters, string[] attachment_Paths)
        {
            var url = baseAddress;
            var json = JsonConvert.SerializeObject(newZohoPurchaseOrderInfo);
            if (parameters == null)
                parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            //var attachment = new string[] { attachment_Path };
            var file = new KeyValuePair<string, string[]>("attachment", attachment_Paths);
            var response = ZohoHttpClient.post(url, getQueryParameters(),parameters,file);
            return PurchaseorderParser.getPurchaseorder(response);
        }

        /// <summary>
        /// Updates the specified purchseorder_id.
        /// </summary>
        /// <param name="purchseorder_id">The purchseorder_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Purchaseorder.</returns>
        public ZohoPurchaseOrder Update(string purchseorder_id,ZohoPurchaseOrder update_info,Dictionary<object,object> parameters,string attachment_path)
        {
            var url = baseAddress + "/" + purchseorder_id;
            var json = JsonConvert.SerializeObject(update_info);
            if (parameters == null)
                parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            var file = new KeyValuePair<string, string>("attachment", attachment_path);
            var response = ZohoHttpClient.put(url,getQueryParameters(),parameters,file);
            return PurchaseorderParser.getPurchaseorder(response);
        }

        /// <summary>
        /// Deletes the specified purchaseorder_id.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <returns>System.String.</returns>
        public string Delete(string purchaseorder_id)
        {
            var url = baseAddress + "/" + purchaseorder_id;
            var response = ZohoHttpClient.delete(url, getQueryParameters());
            return PurchaseorderParser.getMessage(response);
        }

        /// <summary>
        /// Marks as open.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <returns>System.String.</returns>
        public string MarkAsOpen(string purchaseorder_id)
        {
            var url = baseAddress + "/" + purchaseorder_id + "/status/open";
            var response = ZohoHttpClient.post(url, getQueryParameters());
            return PurchaseorderParser.getMessage(response);
        }

        /// <summary>
        /// Marks as billed.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <returns>System.String.</returns>
        public string MarkAsBilled(string purchaseorder_id)
        {
            var url = baseAddress + "/" + purchaseorder_id + "/status/billed";
            var response = ZohoHttpClient.post(url, getQueryParameters());
            return PurchaseorderParser.getMessage(response);
        }

        /// <summary>
        /// Cancels the purchaseorder.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <returns>System.String.</returns>
        public string CancelPurchaseorder(string purchaseorder_id)
        {
            var url = baseAddress + "/" + purchaseorder_id + "/status/cancelled";
            var response = ZohoHttpClient.post(url, getQueryParameters());
            return PurchaseorderParser.getMessage(response);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="zohoEmailDetails">The zohoEmailDetails.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string SendEmail(string purchaseorder_id,ZohoEmailNotification zohoEmailDetails,Dictionary<object,object> parameters)
        {
            var url = baseAddress + "/" + purchaseorder_id + "/email";
            var json = JsonConvert.SerializeObject(zohoEmailDetails);
            if (parameters == null)
                parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            var response = ZohoHttpClient.post(url, getQueryParameters(parameters));
            return PurchaseorderParser.getMessage(response);
        }

        /// <summary>
        /// Gets the content of the email.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ZohoEmail.</returns>
        public ZohoEmail GetEmailContent(string purchaseorder_id, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/email";
            var response = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return PurchaseorderParser.getEmailContent(response);
        }

        /// <summary>
        /// Updates the billing ZohoAddress.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <returns>ZohoAddress.</returns>
        public ZohoAddress UpdateBillingAddress(string purchaseorder_id,ZohoAddress update_info)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/ZohoAddress/billing";
            var json = JsonConvert.SerializeObject(update_info);
            var parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            var response = ZohoHttpClient.put(url, getQueryParameters(parameters));
            return PurchaseorderParser.getAddress(response);
        }

        /// <summary>
        /// Gets the templates.
        /// </summary>
        /// <returns>ZohoTemplateList.</returns>
        public ZohoTemplateList GetTemplates()
        {
            var url = baseAddress + "/templates";
            var response = ZohoHttpClient.get(url, getQueryParameters());
            return PurchaseorderParser.getTemplateList(response);
        }

        /// <summary>
        /// Updates the template.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="template_id">The template_id.</param>
        /// <returns>System.String.</returns>
        public string UpdateTemplate(string purchaseorder_id, string template_id)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/templates/" + template_id;
            var response = ZohoHttpClient.put(url, getQueryParameters());
            return PurchaseorderParser.getMessage(response);
        }

//----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the attachment.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string GetAttachment(string purchaseorder_id, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/attachment";
            ZohoHttpClient.getFile(url, getQueryParameters(parameters));
            return "The attchment is stored at home directory.";
        }

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="attachment_Path">The attachment_ path.</param>
        /// <returns>System.String.</returns>
        public string AddAttachment(string purchaseorder_id, string attachment_Path)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/attachment";
            var attachment = new string[] { attachment_Path };
            var file = new KeyValuePair<string, string[]>("attachment", attachment);
            var responce = ZohoHttpClient.post(url, getQueryParameters(), null, file);
            return PurchaseorderParser.getMessage(responce);
        }

        /// <summary>
        /// Updates the attachment preference.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string UpdateAttachmentPreference(string purchaseorder_id, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/attachment";
            var response = ZohoHttpClient.put(url, getQueryParameters(parameters));
            return PurchaseorderParser.getMessage(response);
        }

        /// <summary>
        /// Deletes an attachment.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <returns>System.String.</returns>
        public string DeleteAnAttachment(string purchaseorder_id)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/attachment";
            var response = ZohoHttpClient.delete(url, getQueryParameters());
            return PurchaseorderParser.getMessage(response);
        }

        //--------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the comments.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <returns>ZohoCommentList.</returns>
        public ZohoCommentList GetComments(string purchaseorder_id)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/comments";
            var response = ZohoHttpClient.get(url, getQueryParameters());
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            return PurchaseorderParser.getCommentList(response);
        }

        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="newZohoCommentInfo">The newZohoCommentInfo.</param>
        /// <returns>ZohoComment.</returns>
        public ZohoComment AddComment(string purchaseorder_id,ZohoComment newZohoCommentInfo)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/comments";
            var json = JsonConvert.SerializeObject(newZohoCommentInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var response = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            return PurchaseorderParser.getComment(response);
        }

        /// <summary>
        /// Updates the comment.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="comment_id">The comment_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <returns>ZohoComment.</returns>
        public ZohoComment UpdateComment(string purchaseorder_id, string comment_id, ZohoComment update_info)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/comments/" + comment_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var response = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            return PurchaseorderParser.getComment(response);
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="purchaseorder_id">The purchaseorder_id.</param>
        /// <param name="comment_id">The comment_id.</param>
        /// <returns>System.String.</returns>
        public string DeleteComment(string purchaseorder_id, string comment_id)
        {
            string url = baseAddress + "/" + purchaseorder_id + "/comments/" + comment_id;
            var response = ZohoHttpClient.delete(url, getQueryParameters());
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            return PurchaseorderParser.getMessage(response);
        }

    }
}
