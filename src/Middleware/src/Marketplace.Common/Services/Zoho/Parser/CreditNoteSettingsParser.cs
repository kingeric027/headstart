using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of CreditnoteSettingsApi.
    /// </summary>
    class CreditnoteSettingsParser
    {
        
        internal static ZohoCreditNoteSettings getCreditNoteSettings(HttpResponseMessage response)
        {
            var creditNoteSettings = new ZohoCreditNoteSettings();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("creditnote_settings"))
            {
                creditNoteSettings = JsonConvert.DeserializeObject<ZohoCreditNoteSettings>(jsonObj["creditnote_settings"].ToString());
            }
            return creditNoteSettings;
        }
    }
}
