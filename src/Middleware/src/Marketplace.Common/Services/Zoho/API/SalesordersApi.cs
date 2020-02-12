using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Parser;
using Marketplace.Common.Services.Zoho.Util;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.API
{
    /// <summary>
    /// Class SalesordersApi.
    /// </summary>
    public class SalesordersApi:Api
    {
        /// <summary>
        /// The base ZohoAddress
        /// </summary>
        static string baseAddress = baseurl + "/salesorders";
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesordersApi" /> class.
        /// </summary>
        /// <param name="auth_token">The auth_token.</param>
        /// <param name="organization_Id">The organization_ identifier.</param>
        public SalesordersApi(string auth_token, string organization_Id)
            : base(auth_token, organization_Id)
        {

        }
        /// <summary>
        /// Gets the salesorders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ZohoSalesorderList.</returns>
        public ZohoSalesorderList GetSalesorders(Dictionary<object, object> parameters)
        {
            string url = baseAddress;
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return SalesorderParser.getSalesorderList(responce);
        }
        /// <summary>
        /// Gets the specified salesorder_id.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ZohoSalesorder.</returns>
        public ZohoSalesorder Get(string salesorder_id,Dictionary<object,object> parameters)
        {
            string url = baseAddress + "/" + salesorder_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return SalesorderParser.getSalesorder(responce);
        }
        /// <summary>
        /// Creates the specified newZohoSalesorderInfo.
        /// </summary>
        /// <param name="newZohoSalesorderInfo">The newZohoSalesorderInfo.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public ZohoSalesorder Create(ZohoSalesorder newZohoSalesorderInfo,Dictionary<object,object> parameters)
        {
            string url = baseAddress;
            var json = JsonConvert.SerializeObject(newZohoSalesorderInfo);
            if (parameters == null)
                parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(parameters));
            return SalesorderParser.getSalesorder(responce);
        }
        /// <summary>
        /// Updates the specified salesorder_id.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ZohoSalesorder.</returns>
        public ZohoSalesorder Update(string salesorder_id,ZohoSalesorder update_info,Dictionary<object,object> parameters)
        {
            string url = baseAddress + "/" + salesorder_id;
            var json = JsonConvert.SerializeObject(update_info);
            if (parameters == null)
                parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(parameters));
            return SalesorderParser.getSalesorder(responce);
        }
        /// <summary>
        /// Deletes the specified salesorder_id.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <returns>System.String.</returns>
        public string Delete(string salesorder_id)
        {
            string url = baseAddress + "/" + salesorder_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return SalesorderParser.getMessage(responce);
        }
        /// <summary>
        /// Marks as open.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <returns>System.String.</returns>
        public string MarkAsOpen(string salesorder_id)
        {
            string url = baseAddress + "/" + salesorder_id + "/status/open";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return SalesorderParser.getMessage(responce);
        }
        /// <summary>
        /// Marks as void.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <returns>System.String.</returns>
        public string MarkAsVoid(string salesorder_id)
        {
            string url = baseAddress + "/" + salesorder_id + "/status/void";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return SalesorderParser.getMessage(responce);
        }
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="zohoEmailDetails">The email_datails.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string SendEmail(string salesorder_id,ZohoEmailNotification zohoEmailDetails,Dictionary<object,object> parameters)
        {
            string url = baseAddress + "/" + salesorder_id + "/email";
            var json=JsonConvert.SerializeObject(zohoEmailDetails);
            if(parameters==null)
                parameters=new Dictionary<object,object>();
            parameters.Add("JSONString",json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(parameters));
            return SalesorderParser.getMessage(responce);
        }
        /// <summary>
        /// Gets the content of the email.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ZohoEmail.</returns>
        public ZohoEmail GetEmailContent(string salesorder_id,Dictionary<object,object> parameters)
        {
            string url = baseAddress + "/" + salesorder_id + "/email";
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return SalesorderParser.getEmailContent(responce);
        }
        /// <summary>
        /// Bulks the export salesorders.
        /// </summary>
        /// <param name="salesorder_ids">The salesorder_ids.</param>
        /// <returns>System.String.</returns>
        public string BulkExportSalesorders(List<string> salesorder_ids)
        {
            string url = baseAddress + "/pdf";
            string salesorderIds=null;
            foreach(var salesorderId in salesorder_ids)
                salesorderIds=salesorderIds+salesorderId;
            var parameters=new Dictionary<object,object>();
            parameters.Add("salesorder_ids",salesorderIds);
            ZohoHttpClient.getFile(url, getQueryParameters(parameters));
            return "The Salesorders Exported to pdf Successfully.";
        }

        /// <summary>
        /// Bulks the print salesorders.
        /// </summary>
        /// <param name="salesorder_ids">The salesorder_ids.</param>
        /// <returns>System.String.</returns>
        public string BulkPrintSalesorders(List<string> salesorder_ids)
        {
            string url = baseAddress + "/print";
            string salesorderIds = null;
            foreach (var salesorderId in salesorder_ids)
                salesorderIds = salesorderIds + salesorderId;
            var parameters = new Dictionary<object, object>();
            parameters.Add("salesorder_ids", salesorderIds);
            ZohoHttpClient.getFile(url, getQueryParameters(parameters));
            return "The Salesorders Printed Successfully.";
        }

        /// <summary>
        /// Updates the billing ZohoAddress.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <returns>System.String.</returns>
        public ZohoAddress UpdateBillingAddress(string salesorder_id,ZohoAddress update_info)
        {
            string url = baseAddress + "/" + salesorder_id + "/address/billing";
            var json = JsonConvert.SerializeObject(update_info);
            var parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            var response = ZohoHttpClient.put(url, getQueryParameters(parameters));
            return SalesorderParser.getAddress(response);
        }
        /// <summary>
        /// Updates the shipping ZohoAddress.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <returns>System.String.</returns>
        public ZohoAddress UpdateShippingAddress(string salesorder_id, ZohoAddress update_info)
        {
            string url = baseAddress + "/" + salesorder_id + "/address/shipping";
            var json = JsonConvert.SerializeObject(update_info);
            var parameters = new Dictionary<object, object>();
            parameters.Add("JSONString", json);
            var response = ZohoHttpClient.put(url, getQueryParameters(parameters));
            return SalesorderParser.getAddress(response);
        }
        /// <summary>
        /// Gets the templates.
        /// </summary>
        /// <returns>ZohoTemplateList.</returns>
        public ZohoTemplateList GetTemplates()
        {
            string url = baseAddress + "/templates";
            var response = ZohoHttpClient.get(url, getQueryParameters());
            return SalesorderParser.getTemplateList(response);
        }
        /// <summary>
        /// Updates the template.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="template_id">The template_id.</param>
        /// <returns>System.String.</returns>
        public string UpdateTemplate(string salesorder_id,string template_id)
        {
            string url = baseAddress + "/" + salesorder_id + "/templates/" + template_id;
            var response = ZohoHttpClient.put(url, getQueryParameters());
            return SalesorderParser.getMessage(response);
        }
//---------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the attachment.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string GetAttachment(string salesorder_id,Dictionary<object,object> parameters)
        {
            string url = baseAddress + "/" + salesorder_id + "/attachment";
            ZohoHttpClient.getFile(url, getQueryParameters(parameters));
            return "The attchment is stored at home directory.";
        }

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="attachment_Path">The attachment_ path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string AddAttachment(string salesorder_id, string attachment_Path, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + salesorder_id + "/attachment";
            var attachment = new string[] { attachment_Path };
            var file = new KeyValuePair<string, string[]>("attachment", attachment);
            var responce = ZohoHttpClient.post(url, getQueryParameters(parameters), null, file);
            return SalesorderParser.getMessage(responce);
        }

        /// <summary>
        /// Updates the attachment preference.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string UpdateAttachmentPreference(string salesorder_id,Dictionary<object,object> parameters)
        {
            string url = baseAddress + "/" + salesorder_id + "/attachment";
            var response = ZohoHttpClient.put(url, getQueryParameters(parameters));
            return SalesorderParser.getMessage(response);
        }

        /// <summary>
        /// Deletes an attachment.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <returns>System.String.</returns>
        public string DeleteAnAttachment(string salesorder_id)
        {
            string url = baseAddress + "/" + salesorder_id + "/attachment";
            var response = ZohoHttpClient.delete(url, getQueryParameters());
            return SalesorderParser.getMessage(response);
        }

//----------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the comments.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <returns>ZohoCommentList.</returns>
        public ZohoCommentList GetComments(string salesorder_id)
        {
            string url = baseAddress + "/" + salesorder_id + "/comments";
            var response = ZohoHttpClient.get(url, getQueryParameters());
            return SalesorderParser.getCommentList(response);
        }

        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="newZohoCommentInfo">The newZohoCommentInfo.</param>
        /// <returns>ZohoComment.</returns>
        public ZohoComment AddComment(string salesorder_id,ZohoComment newZohoCommentInfo)
        {
            string url = baseAddress + "/" + salesorder_id + "/comments";
            var json = JsonConvert.SerializeObject(newZohoCommentInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var response = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return SalesorderParser.getComment(response);
        }

        /// <summary>
        /// Updates the comment.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="comment_id">The comment_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <returns>ZohoComment.</returns>
        public ZohoComment UpdateComment(string salesorder_id,string comment_id,ZohoComment update_info)
        {
            string url = baseAddress + "/" + salesorder_id + "/comments/" + comment_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var response = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return SalesorderParser.getComment(response);
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="salesorder_id">The salesorder_id.</param>
        /// <param name="comment_id">The comment_id.</param>
        /// <returns>System.String.</returns>
        public string DeleteComment(string salesorder_id,string comment_id)
        {
            string url = baseAddress + "/" + salesorder_id + "/comments/" + comment_id;
            var response = ZohoHttpClient.delete(url, getQueryParameters());
            return SalesorderParser.getMessage(response);
        }
    }
}
