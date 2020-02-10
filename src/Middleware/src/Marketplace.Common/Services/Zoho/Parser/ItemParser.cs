using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of ItemsApi.
    /// </summary>
    class ItemParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoItemList getItemList(HttpResponseMessage response)
        {
            var itemList = new ZohoItemList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("items"))
            {
                var itemsArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["items"].ToString());
                foreach(var itemObj in itemsArray)
                {
                    var item = new ZohoLineItem();
                    item = JsonConvert.DeserializeObject<ZohoLineItem>(itemObj.ToString());
                    itemList.Add(item);
                }
            }
            if (jsonObj.ContainsKey("page_context"))
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jsonObj["page_context"].ToString());
                itemList.ZohoPageContext = pageContext;
            }
            return itemList;
        }

        internal static ZohoLineItem getItem(HttpResponseMessage response)
        {
            var item = new ZohoLineItem();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("item"))
            {
                item = JsonConvert.DeserializeObject<ZohoLineItem>(jsonObj["item"].ToString());
            }
            return item;
        }
    }
}
