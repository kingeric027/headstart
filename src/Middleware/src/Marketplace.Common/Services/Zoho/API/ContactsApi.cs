using System;
using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Parser;
using Marketplace.Common.Services.Zoho.Util;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.API
{
    /// <summary>
    /// ContactsApi is used to<br></br>
    ///     Create a new zohoContact to the ZohoOrganization,<br></br>
    ///     Get the list of contacts,<br></br>
    ///     Get or update the specified zohoContact,<br></br>
    ///     Mark zohoContact as active or inactive,<br></br>
    ///     Enable or disable automated payment reminders for a zohoContact,<br></br>
    ///     Activate or deactivate the track1099,<br></br>
    ///     Send ZohoEmail or statement to the zohoContact,<br></br>
    ///     List of comments and refund history of a zohoContact,<br></br>
    ///     Delete the zohoContact.<br></br>
    /// </summary>
    public class ContactsApi:Api
    {
        static string baseAddress =baseurl + "/contacts";
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactsApi" /> class.
        /// </summary>
        /// <param name="auth_token">The auth_token is used for the authentication purpose.</param>
        /// <param name="organization_Id">The organization_ identifier is used to define the current working organisation.</param>
        public ContactsApi(string auth_token, string organization_Id)
            : base(auth_token, organization_Id)
        {

        }

        /// <summary>
        /// List all contacts with pagination.
        /// </summary>
        /// <param name="parameters">The parameters is the dictionary object which is having the filters to refine the list in the form of key,value pairs.<br></br>The possible filter keys and variants are listed below<br></br>
        /// <table>
        /// <tr><td>contact_name</td><td>Search contacts by zohoContact name.<br></br>Variants: <i>contact_name_startswith</i> and <i>contact_name_contains</i></td></tr>
        /// <tr><td>company_name</td><td>Search contacts by company name.<br></br>Variants: <i>company_name_startswith</i> and <i>company_name_contains</i></td></tr>
        /// <tr><td>first_name</td><td>Search contacts by first name of the zohoContact person.<br></br>Variants: <i>first_name_startswith</i> and <i>first_name_contains</i></td></tr>
        /// <tr><td>last_name</td><td>Search contacts by last name of the zohoContact person.<br></br>Variants: <i>last_name_startswith</i> and <i>last_name_contains</i></td></tr>
        /// <tr><td>ZohoAddress</td><td>Search contacts by any of the ZohoAddress fields.<br></br>Variants: <i>address_startswith</i> and <i>address_contains</i></td></tr>
        /// <tr><td>email</td><td>Search contacts by email of the zohoContact person.<br></br>Variants: <i>email_startswith</i> and <i>email_contains</i></td></tr>
        /// <tr><td>phone</td><td>Search contacts by phone number of the zohoContact person.<br></br>Variants: <i>phone_startswith</i> and <i>phone_contains</i></td></tr>
        /// <tr><td>filter_by</td><td>Filter contacts by status.<br></br>Allowed Values: <i>FourOverStatus.All, FourOverStatus.Active, FourOverStatus.Inactive, FourOverStatus.Duplicate, FourOverStatus.Customers, FourOverStatus.Vendors</i> and <i>FourOverStatus.Crm</i></td></tr>
        /// <tr><td>search_text</td><td>Search contacts by zohoContact name or notes.</td></tr>
        /// <tr><td>sort_column</td><td>Sort contacts.<br></br>Allowed Values: <i>contact_name, first_name, last_name, email, outstanding_receivable_amount, outstanding_payable_amount, created_time</i> and <i>last_modified_time</i></td></tr>
        /// </table>
        /// </param>
        /// <returns>ZohoContactList object.</returns>
        public ZohoContactList GetContacts(Dictionary<object, object> parameters)
        {
            string url = baseAddress;
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return ContactParser.getContactList(responce);
        }

        /// <summary>
        /// Gets details of a specified zohoContact.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifer of the zohoContact for which going to get the details.</param>
        /// <returns>ZohoContact object.</returns>
        public ZohoContact Get(string contact_id)
        {
            string url = baseAddress+"/"+contact_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            Console.WriteLine(responce.Content.ReadAsStringAsync().Result);
            return ContactParser.getContact(responce);
        }

        /// <summary>
        /// Creates a zohoContact with given information.
        /// </summary>
        /// <param name="newZohoContactInfo">The newZohoContactInfo is the ZohoContact object which provides the information to create a zohoContact with contact_name as mandatory parameters.</param>
        /// <returns>ZohoContact object.</returns>
        public ZohoContact Create(ZohoContact newZohoContactInfo)
        {
            string url = baseAddress;
            var json = JsonConvert.SerializeObject(newZohoContactInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ContactParser.getContact(responce); 
        }

        /// <summary>
        /// Update an existing zohoContact. To delete a zohoContact person remove it from the contact_persons list.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <param name="update_info">The update_info is the ZohoContact object which contains the update information.</param>
        /// <returns>ZohoContact object.</returns>
        public ZohoContact Update(string contact_id, ZohoContact update_info)
        {
            string url = baseAddress + "/" + contact_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return ContactParser.getContact(responce);
        }

        /// <summary>
        /// Deletes an existing zohoContact.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>System.String.<br></br>The success message is "The zohoContact has been deleted."</returns>
        public string Delete(string contact_id)
        {
            string url = baseAddress + "/" + contact_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// Marks a zohoContact as active.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>System.String.<br></br>The success message is "The zohoContact has been marked as active."</returns>
        public string MarkAsActive(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/active";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// Marks a zohoContact as inactive.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>System.String.<br></br>The success message is "The zohoContact has been marked as inactive."</returns>
        public string MarkAsInactive(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/inactive";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// Enables automated payment reminders for a zohoContact.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>System.String.<br></br>The success message is "All reminders associated with this zohoContact have been enabled."</returns>
        public string EnablePaymentReminder(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/paymentreminder/enable";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }


        /// <summary>
        /// Disables automated payment reminders for a zohoContact.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>System.String.<br></br>The success message is "All reminders associated with this zohoContact have been stopped."</returns>
        public string DisablePaymentReminder(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/paymentreminder/disable";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// ZohoEmail statement to the zohoContact (If the ZohoEmail object is not given, email will be sent with the default email content).
        /// </summary>
        /// <param name="contact_id">The contact_id the identifier of the zohoContact.</param>
        /// <param name="zohoEmailNotifyDetails">The email notify_details is the ZohoEmailNotification object contains the email details with to_mail_ids,subject and body parameters as mandatory.</param>
        /// <param name="attachment_paths">The attachment_paths is the file details which is going to be attached to the mail.</param>
        /// <param name="parameters">The parameters is the dictionary object which is optionally had the following key value pairs.<br></br>
        /// <table>
        /// <tr><td>start_date</td><td>If start_date and end_date are not given, current month's statement will be sent to zohoContact.</td></tr>
        /// <tr><td>end_date</td><td>End date for the statement.</td></tr>
        /// </table>
        /// </param>
        /// <returns>System.String.<br></br>The success message is "ZohoStatement has been sent to the customer." </returns>
        public string SendEmailStatement(string contact_id, ZohoEmailNotification zohoEmailNotifyDetails, string[] attachment_paths, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + contact_id + "/statements/email";
            var json = JsonConvert.SerializeObject(zohoEmailNotifyDetails);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var files = new KeyValuePair<string, string[]>("attachments", attachment_paths);
            var responce = ZohoHttpClient.post(url, getQueryParameters(),jsonstring,files);
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// Gets the content of the email statement.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <param name="parameters">The parameters is the dictionary object which is optionally had the following key value pairs.<br></br>
        /// <table>
        /// <tr><td>start_date</td><td>If start_date and end_date are not given, current month's statement will be sent to zohoContact.</td></tr>
        /// <tr><td>end_date</td><td>End date for the statement.</td></tr>
        /// </table>
        /// </param>
        /// <returns>ZohoEmail object.</returns>
        public ZohoEmail GetEmailStatementContent(string contact_id, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + contact_id + "/statements/email";
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return ContactParser.getEmailContent(responce);
        }

        /// <summary>
        /// Sends email to zohoContact.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <param name="zohoEmailContent">The zohoEmailContent.</param>
        /// <param name="parameters">The parameters contains the query string in the form of key-value pair.<br></br>The possible key as mentioned below: <br></br>br>send_customer_statement - Send customer statement pdf with email. <br></br></param>
        /// <param name="attachment_paths">The attachment_paths are the attached files information.</param>
        /// <returns>System.String.<br></br>The success message is "ZohoEmail has been sent."</returns>
        public string SendEmailContact(string contact_id, ZohoEmailNotification zohoEmailContent, Dictionary<object, object> parameters, string[] attachment_paths)
        {
            string url = baseAddress + "/" + contact_id + "/email";
            var json = JsonConvert.SerializeObject(zohoEmailContent);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var files = new KeyValuePair<string, string[]>("attachments", attachment_paths);
            var responce = ZohoHttpClient.post(url, getQueryParameters(),jsonstring, files);
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// List recent activities of a zohoContact.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>ZohoCommentList object.</returns>
        public ZohoCommentList GetComments(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/comments";
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ContactParser.getCommentList(responce);
        }

        /// <summary>
        /// List the refund history of a zohoContact.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>CreditnoteRefundList object.</returns>
        public ZohoCreditNoteRefundList GetRefunds(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/refunds";
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return ContactParser.getCreditNoteRefundList(responce);
        }

        /// <summary>
        /// Track a zohoContact for 1099 reporting. (Note: This API is only available when the ZohoOrganization's country is U.S.A)
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>System.String.<br></br>The success message is "1099 tracking is enabled."</returns>
        public string Track1099(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/track1099";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// Use this API to stop tracking payments to a vendor for 1099 reporting. (Note: This API is only available when the ZohoOrganization's country is U.S.A)
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>System.String.<br></br>The success message is "1099 tracking is disabled."</returns>
        public string UnTrack1099(string contact_id)
        {
            string url = baseAddress + "/" + contact_id + "/untrack1099";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }
//--------------------------------------------------------------------------------------------
        /// <summary>
        /// List zohoContact persons of a zohoContact with pagination.
        /// </summary>
        /// <param name="contact_id">The contact_id is the identifier of the zohoContact.</param>
        /// <returns>ZohoContactPersonList object.</returns>
        public ZohoContactPersonList GetContactPersons(string contact_id, Dictionary<object, object> parameters)
        {
            string url = baseAddress + "/" + contact_id + "/contactpersons";
            var responce = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return ContactParser.getContactPersonList(responce);
        }

        /// <summary>
        /// Gets the zohoContact person details.
        /// </summary>
        /// <param name="contact_person_id">The contact_person_id is the identifier of the zohoContact person.</param>
        /// <returns>ZohoContactPerson object.</returns>
        public ZohoContactPerson GetContactPerson(string contact_id,string contact_person_id)
        {
            string url = baseAddress +"/"+contact_id+"/contactpersons/"+contact_person_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            Console.WriteLine(responce.Content.ReadAsStringAsync().Result);
            return ContactParser.getContactPerson(responce);
        }

        /// <summary>
        /// Creates a zohoContact person for zohoContact.
        /// </summary>
        /// <param name="newZohoContactPersonInfo">The newZohoContactPersonInfo is the ZohoContactPerson object which is having the information to create a zohoContact person with contact_id as mandatory parameter.</param>
        /// <returns>ZohoContactPerson object.</returns>
        public ZohoContactPerson CreateContactPerson(ZohoContactPerson newZohoContactPersonInfo)
        {
            string url = baseAddress + "/contactpersons";
            var json = JsonConvert.SerializeObject(newZohoContactPersonInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return ContactParser.getContactPerson(responce);
        }

        /// <summary>
        /// Update an existing zohoContact person.
        /// </summary>
        /// <param name="contact_person_id">The contact_person_id is the identifier of the zohoContact person.</param>
        /// <param name="update_info">The update_info is the ZohoContactPerson object with contact_id as mandatory parameter which contains the changes to be modified.</param>
        /// <returns>ZohoContactPerson object.</returns>
        public ZohoContactPerson UpdateContactperson(string contact_person_id, ZohoContactPerson update_info)
        {
            string url = baseAddress + "/contactpersons/" + contact_person_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return ContactParser.getContactPerson(responce);
        }

        /// <summary>
        /// Deletes an existing zohoContact person.
        /// </summary>
        /// <param name="contact_Person_id">The contact_ person_id is the identifier of the zohoContact person.</param>
        /// <returns>System.String.<br></br>The success message is "The zohoContact person has been deleted."</returns>
        public string DeleteContactPerson(string contact_Person_id)
        {
            string url = baseAddress + "/contactpersons/" + contact_Person_id;
            var responce = ZohoHttpClient.delete(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }

        /// <summary>
        /// Marks a zohoContact person as primary for the zohoContact.
        /// </summary>
        /// <param name="contact_person_id">The contact_person_id is the identifier of the zohoContact person.</param>
        /// <returns>System.String.<br></br>The success message is "This zohoContact person has been marked as your primary zohoContact person."</returns>
        public string MarkAsPrimaryContactPerson(string contact_person_id)
        {
            string url = baseAddress + "/contactpersons/" + contact_person_id + "/primary";
            var responce = ZohoHttpClient.post(url, getQueryParameters());
            return ContactParser.getMessage(responce);
        }

        
    }   
}
