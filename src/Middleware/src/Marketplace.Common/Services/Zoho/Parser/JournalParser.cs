using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of JournalsApiParser.
    /// </summary>
    class JournalParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoJournalList getJournalList(HttpResponseMessage responce)
        {
            var journalList = new ZohoJournalList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("journals"))
            {
                var journalsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["journals"].ToString());
                foreach(var journalObj in journalsArray)
                {
                    var journal = new ZohoJournal();
                    journal = JsonConvert.DeserializeObject<ZohoJournal>(journalObj.ToString());
                    journalList.Add(journal);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                journalList.ZohoPageContext = pageContext;
            }
            return journalList;
        }

        internal static ZohoJournal getJournal(HttpResponseMessage responce)
        {
            var journal = new ZohoJournal();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("journal"))
            {
                journal = JsonConvert.DeserializeObject<ZohoJournal>(jsonObj["journal"].ToString());
            }
            return journal;
        }
    }
}
