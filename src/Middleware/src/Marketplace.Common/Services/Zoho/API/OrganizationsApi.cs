using System;
using System.Collections.Generic;
using Marketplace.Common.Services.Zoho.Models;
using Marketplace.Common.Services.Zoho.Parser;
using Marketplace.Common.Services.Zoho.Util;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.API
{
    /// <summary>
    /// Class OrganizationsApi is used to<br></br>
    /// Create a new ZohoOrganization,<br></br>
    /// Get and update of the ZohoOrganization details,<br></br>
    /// Get the list of organizations for the user.<br></br>
    /// </summary>
    public class OrganizationsApi:Api
    {
        /// <summary>
        /// The base ZohoAddress
        /// </summary>
        static string baseAddress =baseurl + "/organizations";
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationsApi" /> class.
        /// </summary>
        /// <param name="auth_token">The auth_token is used for the authentication purpose.</param>
        /// <param name="organization_Id">The organization_ identifier is used to define the current working organisation.</param>
        public OrganizationsApi(string auth_token, string organization_Id)
            : base(auth_token, organization_Id)
        {

        }
        /// <summary>
        /// Get the list of organizations.
        /// </summary>
        /// <returns>List of ZohoOrganization objects.</returns>
        public ZohoOrganizationList GetOrganizations()
        {
            string url = baseAddress;
            var response = ZohoHttpClient.get(url, getQueryParameters());
            return OrganizationParser.getOrganizationList(response);
        }

        /// <summary>
        /// Get the details of an ZohoOrganization.
        /// </summary>
        /// <param name="organization_id">The organization_id is the identifier of the ZohoOrganization.</param>
        /// <returns>ZohoOrganization object.</returns>
        public ZohoOrganization Get(string organization_id)
        {
            string url = baseAddress + "/" + organization_id;
            var responce = ZohoHttpClient.get(url, getQueryParameters());
            return OrganizationParser.getOrganization(responce);
        }

        /// <summary>
        /// Create an ZohoOrganization.
        /// </summary>
        /// <param name="oranization_info">The oranization_info is the ZohoOrganization object with name,currency_code and time_zone as mandatory attributes.</param>
        /// <returns>ZohoOrganization object.</returns>
        public ZohoOrganization Create(ZohoOrganization oranization_info)
        {
            string url = baseAddress;
            var json = JsonConvert.SerializeObject(oranization_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            Console.WriteLine(responce.Content.ReadAsStringAsync().Result);
            return OrganizationParser.getOrganization(responce);
        }

        /// <summary>
        /// Update the details of an ZohoOrganization.
        /// </summary>
        /// <param name="organization_id">The organization_id is the identifier of the ZohoOrganization.</param>
        /// <param name="update_info">The update_info is the ZohoOrganization object which contains the updation information.</param>
        /// <returns>ZohoOrganization object.</returns>
        public ZohoOrganization Upadte(string organization_id, ZohoOrganization update_info)
        {
            string url = baseAddress + "/" + organization_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return OrganizationParser.getOrganization(responce);
        }
        /// <summary>
        /// Adds the ZohoOrganization ZohoAddress.
        /// </summary>
        /// <param name="zohoAddressInfo">The zohoAddressInfo.</param>
        /// <returns>ZohoAddress.</returns>
        public ZohoAddress AddOrganizationAddress(ZohoAddress zohoAddressInfo)
        {
            string url = baseAddress + "/address";
            var json = JsonConvert.SerializeObject(zohoAddressInfo);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.post(url, getQueryParameters(jsonstring));
            return OrganizationParser.getOrganizationAddress(responce);
        }
        /// <summary>
        /// Updates the ZohoOrganization ZohoAddress.
        /// </summary>
        /// <param name="organization_address_id">The organization_address_id.</param>
        /// <param name="update_info">The update_info.</param>
        /// <returns>ZohoAddress.</returns>
        public ZohoAddress UpdateOrganizationAddress(string organization_address_id,ZohoAddress update_info)
        {
            string url = baseAddress + "/address/" + organization_address_id;
            var json = JsonConvert.SerializeObject(update_info);
            var jsonstring = new Dictionary<object, object>();
            jsonstring.Add("JSONString", json);
            var responce = ZohoHttpClient.put(url, getQueryParameters(jsonstring));
            return OrganizationParser.getOrganizationAddress(responce);
        }
    }
}
