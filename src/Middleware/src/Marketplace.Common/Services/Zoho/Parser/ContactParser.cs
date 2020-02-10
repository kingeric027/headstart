using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of ContactsApi.
    /// </summary>
    class ContactParser
    {
        
        internal static ZohoContactList getContactList(HttpResponseMessage responce)
        {
            var contactList = new ZohoContactList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("contacts"))
            {
                var contactsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["contacts"].ToString());
                foreach(var contactObj in contactsArray)
                {
                    var contact = new ZohoContact();
                    contact = JsonConvert.DeserializeObject<ZohoContact>(contactObj.ToString());
                    contactList.Add(contact);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                contactList.ZohoPageContext = pageContext;
            }
            return contactList;
        }

        internal static ZohoContact getContact(HttpResponseMessage responce)
        {
            var contact = new ZohoContact();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("contact"))
            {
                contact = JsonConvert.DeserializeObject<ZohoContact>(jsonObj["contact"].ToString());
            }
            return contact;
        }

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoEmail getEmailContent(HttpResponseMessage responce)
        {
            var mailContent = new ZohoEmail();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if(jsonObj.ContainsKey("data"))
            {
                mailContent = JsonConvert.DeserializeObject<ZohoEmail>(jsonObj["data"].ToString());
            }
            return mailContent;
        }

        internal static ZohoCommentList getCommentList(HttpResponseMessage responce)
        {
            var commentList = new ZohoCommentList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("contact_comments"))
            {
                var commentsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["contact_comments"].ToString());
                foreach(var commentObj in commentsArray)
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

        internal static ZohoCreditNoteRefundList getCreditNoteRefundList(HttpResponseMessage responce)
        {
            var creditnoteRefundList = new ZohoCreditNoteRefundList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("creditnote_refunds"))
            {
                var creditNotesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["creditnote_refunds"].ToString());
                foreach(var creditNoteObj in creditNotesArray)
                {
                    var creditNote = new ZohoCreditNote();
                    creditNote = JsonConvert.DeserializeObject<ZohoCreditNote>(creditNoteObj.ToString());
                    creditnoteRefundList.Add(creditNote);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                creditnoteRefundList.ZohoPageContext = pageContext;
            }
            return creditnoteRefundList;
        }

        internal static ZohoContactPersonList getContactPersonList(HttpResponseMessage responce)
        {
            var contactPersonList = new ZohoContactPersonList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("contact_persons"))
            {
                var contactPersonsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["contact_persons"].ToString());
                foreach(var contactPersonObj in contactPersonsArray)
                {
                    var contactPerson = new ZohoContactPerson();
                    contactPerson = JsonConvert.DeserializeObject<ZohoContactPerson>(contactPersonObj.ToString());
                    contactPersonList.Add(contactPerson);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                contactPersonList.ZohoPageContext = pageContext;
            }
            return contactPersonList;
        }

        internal static ZohoContactPerson getContactPerson(HttpResponseMessage responce)
        {
            var contactPerson = new ZohoContactPerson();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("contact_person"))
            {
                contactPerson = JsonConvert.DeserializeObject<ZohoContactPerson>(jsonObj["contact_person"].ToString());
            }
            return contactPerson;
        }
    }
}
