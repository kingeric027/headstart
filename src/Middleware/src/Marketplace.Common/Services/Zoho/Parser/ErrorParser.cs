using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    class ErrorParser
    {

        internal static string getErrorMessage(HttpResponseMessage responce)
        {
            var message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if(jsonObj.ContainsKey("message"))
            {
                message = jsonObj["message"].ToString();
            }
            return message;
        }
    }
}
