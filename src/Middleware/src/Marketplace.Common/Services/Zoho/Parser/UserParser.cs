using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of UsersApi.
    /// </summary>
    class UserParser
    {

        internal static string getMessage(HttpResponseMessage responce)
        {
            string message = "";
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("message"))
                message = jsonObj["message"].ToString();
            return message;
        }

        internal static ZohoUserList getUserList(HttpResponseMessage response)
        {
            var userList = new ZohoUserList();
            JObject jobject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            if(jobject["users"]!=null)
            {
                foreach(var userObj in jobject["users"])
                {
                    var user = getUserProperties(userObj.ToString());
                    userList.Add(user);
                }
            }
            if (jobject["page_context"]!=null)
            {
                var pageContext = new ZohoPageContext();
                pageContext = JsonConvert.DeserializeObject<ZohoPageContext>(jobject["page_context"].ToString());
                userList.ZohoPageContext = pageContext;
            }
            return userList;
        }

        private static ZohoUser getUserProperties(string json)
        {
            var user = new ZohoUser();
            var userObj = JObject.Parse(json);
            if(userObj["user_id"]!=null)
            {
                user.user_id = userObj["user_id"].ToString();
            }
            if(userObj["role_id"]!=null)
            {
                user.role_id = userObj["role_id"].ToString();
            }
            if (userObj["name"] != null)
            {
                user.name = userObj["name"].ToString();
            }
            if (userObj["email"] != null)
            {
                user.email = userObj["email"].ToString();
            }
            if (userObj["user_role"] != null)
            {
                user.user_role = userObj["user_role"].ToString();
            }
            if (userObj["status"] != null)
            {
                user.status = userObj["status"].ToString();
            }
            if (userObj["is_current_user"] != null)
            {
                user.is_current_user = (bool)userObj["is_current_user"];
            }
            if (userObj["photo_url"] != null)
            {
                user.photo_url = userObj["photo_url"].ToString();
            }
            return user;
        }
    }
}
