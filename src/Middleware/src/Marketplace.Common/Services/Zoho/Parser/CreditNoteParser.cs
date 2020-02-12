using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of CreditNoteApi.
    /// </summary>
    class CreditNoteParser
    {
        internal static ZohoCreditNoteList getCreditnoteList(HttpResponseMessage responce)
        {
            var creditNoteList = new ZohoCreditNoteList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("creditnotes"))
            {
                var creditnotesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["creditnotes"].ToString());
                foreach(var creditnoteObj in creditnotesArray)
                {
                    var creditnote = new ZohoCreditNote();
                    creditnote = JsonConvert.DeserializeObject<ZohoCreditNote>(creditnoteObj.ToString());
                    creditNoteList.Add(creditnote);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                creditNoteList.ZohoPageContext = pageContext;
            }
            return creditNoteList;
        }

        internal static ZohoCreditNote getCreditnote(HttpResponseMessage responce)
        {
            var creditnote = new ZohoCreditNote();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("creditnote"))
            {
                creditnote = JsonConvert.DeserializeObject<ZohoCreditNote>(jsonObj["creditnote"].ToString());
            }
            return creditnote;
        }

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoEmailHistoryList getEmailHistoryList(HttpResponseMessage responce)
        {
            var emailHistoryList = new ZohoEmailHistoryList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("email_history"))
            {
                var emailHistoryArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["email_history"].ToString());
                foreach(var emailHistoryObj in emailHistoryArray)
                {
                    var emailHistory = new ZohoEmailHistory();
                    emailHistory = JsonConvert.DeserializeObject<ZohoEmailHistory>(emailHistoryObj.ToString());
                    emailHistoryList.Add(emailHistory);
                }
            }
            return emailHistoryList;
        }

        internal static ZohoTemplateList getTemplateList(HttpResponseMessage responce)
        {
            var templateList = new ZohoTemplateList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("templates"))
            {
                var templatesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["templates"].ToString());
                foreach(var templateObj in templatesArray)
                {
                    var template = new ZohoTemplate();
                    template = JsonConvert.DeserializeObject<ZohoTemplate>(templateObj.ToString());
                    templateList.Add(template);
                }
            }
            return templateList;
        }

        internal static ZohoCreditedInvoiceList getCreditedInvoiceList(HttpResponseMessage responce)
        {
            var creditedInvoceList = new ZohoCreditedInvoiceList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("invoices_credited"))
            {
                var creditedInvoicesArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["invoices_credited"].ToString());
                foreach(var invoiceObj in creditedInvoicesArray)
                {
                    var creditedInvoice = new ZohoCreditedInvoice();
                    creditedInvoice = JsonConvert.DeserializeObject<ZohoCreditedInvoice>(invoiceObj.ToString());
                    creditedInvoceList.Add(creditedInvoice);
                }
            }
            return creditedInvoceList;
        }

        internal static ZohoCreditedInvoiceList getCreditsAppliedInvoices(HttpResponseMessage responce)
        {
            var creditedInvoiceList = new ZohoCreditedInvoiceList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("apply_to_invoices"))
            {
                var applyToInvoiceObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObj["apply_to_invoices"].ToString());
                if(applyToInvoiceObj.ContainsKey("invoices"))
                {
                    var invoicesArray = JsonConvert.DeserializeObject<List<object>>(applyToInvoiceObj["invoices"].ToString());
                    foreach(var invoiceObj in invoicesArray)
                    {
                        var creditedinvoice = new ZohoCreditedInvoice();
                        creditedinvoice = JsonConvert.DeserializeObject<ZohoCreditedInvoice>(invoiceObj.ToString());
                        creditedInvoiceList.Add(creditedinvoice);
                    }
                }
            }
            return creditedInvoiceList;
        }

        internal static ZohoCreditNoteRefundList getCreditnoteRefundList(HttpResponseMessage responce)
        {
            var creditnoterefundList = new ZohoCreditNoteRefundList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("creditnote_refunds"))
            {
                var refundsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["creditnote_refunds"].ToString());
                foreach(var refundObj in refundsArray)
                {
                    var creditnote = new ZohoCreditNote();
                    creditnote = JsonConvert.DeserializeObject<ZohoCreditNote>(refundObj.ToString());
                    creditnoterefundList.Add(creditnote);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                creditnoterefundList.ZohoPageContext = pageContext;
            }
            return creditnoterefundList;
        }

        internal static ZohoCreditNote getCreditnoteRefund(HttpResponseMessage responce)
        {
            var creditnote = new ZohoCreditNote();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("creditnote_refund"))
            {
                creditnote = JsonConvert.DeserializeObject<ZohoCreditNote>(jsonObj["creditnote_refund"].ToString());
            }
            return creditnote;
        }

        internal static ZohoCommentList getCommentList(HttpResponseMessage responce)
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
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                commentList.ZohoPageContext = pageContext;
            }
            return commentList;
        }

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
