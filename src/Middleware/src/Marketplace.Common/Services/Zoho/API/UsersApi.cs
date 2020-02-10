using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Parser;
using Marketplace.Common.Services.Zoho.Util;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.API
{
    /// <summary>
    /// Class UsersApi is used to <br></br>
    ///     Create the users for the ZohoOrganization,<br></br>
    ///     Get and update the specified user details,<br></br>
    ///     Get the list of users,<br></br>
    ///     Get the current user for the ZohoOrganization,<br></br>
    ///     Invite a new user to the ZohoOrganization,<br></br>
    ///     Change the status of the user to active or inactive,<br></br>
    ///     Delete the specified user from the ZohoOrganization.<br></br>
    /// </summary>
    public class UsersApi:Api
    {
        static string baseAddress = baseurl + "/users";
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersApi"/> class.
        /// </summary>
        /// <param name="auth_token">The auth_token is used for the authentication purpose.</param>
        /// <param name="organization_Id">The organization_ id is used to define the current working organisation.</param>
        public UsersApi(string auth_token, string organization_Id)
            : base(auth_token, organization_Id)
        {

        }

        /// <summary>
        /// Get the list of all users in the ZohoOrganization.
        /// </summary>
        /// <param name="parameters">The parameters is the Dictionary object which conrains the filters in the form of key,value pair to refine the list.<br></br>The possible filters are listed below<br></br>
        /// <table>
        /// <tr><td>filter_by</td><td>Filter through users with user status.<br></br>Allowed Values: <i>FourOverStatus.All, FourOverStatus.Active, FourOverStatus.Inactive, FourOverStatus.Invited</i> and <i>FourOverStatus.Deleted</i></td></tr>
        /// <tr><td>sort_column</td><td>Sort users.<br></br>Allowed Values: <i>name, email, user_role</i> and <i>status</i></td></tr>
        /// </table>
        /// </param>
        /// <returns>ZohoUserList object.</returns>
        public ZohoUserList GetUsers(Dictionary<object, object> parameters)
        {
            string url = baseAddress;
            var response = ZohoHttpClient.get(url, getQueryParameters(parameters));
            return UserParser.getUserList(response);
        }

        /// <summary>
        /// Gets the details of a user.
        /// </summary>
        /// <param name="user_id">The user_id is the identifier of the user.</param>
        /// <returns>ZohoUser object.</returns>
        public ZohoUser Get(string user_id)
        {
            string url = baseAddress + "/" + user_id;
            var response = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.getUser(response);
        }

        /// <summary>
        /// Gets the details of the current user.
        /// </summary>
        /// <returns>ZohoUser object.</returns>
        public ZohoUser GetCurrentUser()
        {
            string url = baseAddress + "/me";
            var response = ZohoHttpClient.get(url, getQueryParameters());
            return ProjectParser.getUser(response);
        }

        /// <summary>
        /// Creates a user for the ZohoOrganization.
        /// </summary>
        /// <param name="zohoUserInfo">The zohoUserInfo is the user object with name and email as mandatory attributes.</param>
        /// <returns>ZohoUser object.</returns>
        public ZohoUser Create(ZohoUser zohoUserInfo)
        {
            string url = baseAddress ;
            var json = JsonConvert.SerializeObject(zohoUserInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var response=ZohoHttpClient.post(url,getQueryParameters(jsonstring));
            return ProjectParser.getUser(response);
        }

        /// <summary>
        /// Update the details of a user.
        /// </summary>
        /// <param name="user_id">The user_id is the identifier of the user.</param>
        /// <param name="update_info">The update_info is the ZohoUser obect which contains the updation information.</param>
        /// <returns>ZohoUser object.</returns>
        public ZohoUser Update(string user_id,ZohoUser update_info)
        {
            string url = baseAddress+"/"+user_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var response = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return ProjectParser.getUser(response);
        }

        /// <summary>
        /// Deletes a user associated to the ZohoOrganization.
        /// </summary>
        /// <param name="user_id">The user_id is the identifier of the user.</param>
        /// <returns>System.String.<br></br>The success message is "The user has been removed from your ZohoOrganization."</returns>
        public string Delete(string user_id)
        {
            string url = baseAddress + "/" + user_id;
            var response = ZohoHttpClient.delete(url, getQueryParameters());
            return UserParser.getMessage(response);
        }

        /// <summary>
        /// Send invitation email to a user.
        /// </summary>
        /// <param name="user_id">The user_id is the identifier of the user.</param>
        /// <returns>System.String.<br></br>The success message is "Your invitation has been sent."</returns>
        public string InviteUser(string user_id)
        {
            string url = baseAddress + "/" + user_id + "/invite";
            var response = ZohoHttpClient.post(url, getQueryParameters());
            return UserParser.getMessage(response);
        }

        /// <summary>
        /// Mark an inactive user as active.
        /// </summary>
        /// <param name="user_id">The user_id is the identifier of the user.</param>
        /// <returns>System.String.<br></br>The success message is "The user has been marked as active."</returns>
        public string MarkAsActive(string user_id)
        {
            string url = baseAddress + "/" + user_id + "/active";
            var response = ZohoHttpClient.post(url, getQueryParameters());
            return UserParser.getMessage(response);
        }

        /// <summary>
        /// Mark an active user as inactive.
        /// </summary>
        /// <param name="user_id">The user_id is the identifier of the user.</param>
        /// <returns>System.String.<br></br>The success message is "The user has been marked as inactive."</returns>
        public string MarkAsInactive(string user_id)
        {
            string url = baseAddress + "/" + user_id + "/inactive";
            var response = ZohoHttpClient.post(url, getQueryParameters());
            return UserParser.getMessage(response);
        }
    }
}
