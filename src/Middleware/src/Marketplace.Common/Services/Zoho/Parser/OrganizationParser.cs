using System.Collections.Generic;
using System.Net.Http;
using Marketplace.Common.Services.Zoho.Models;
using Newtonsoft.Json;

namespace Marketplace.Common.Services.Zoho.Parser
{
    /// <summary>
    /// Used to define the parser object of OrganizationsApi.
    /// </summary>
    class OrganizationParser
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public int code { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string message { get; set; }
        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>The organizations.</value>
        public List<ZohoOrganization> organizations { get; set; }
        /// <summary>
        /// Gets or sets the ZohoOrganization.
        /// </summary>
        /// <value>The ZohoOrganization.</value>
        public ZohoOrganization ZohoOrganization { get; set; }

        internal static ZohoOrganizationList getOrganizationList(HttpResponseMessage response)
        {
            var organizationList = new ZohoOrganizationList();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("organizations"))
            {
                var organizationArray = JsonConvert.DeserializeObject<List<object>>(jsonObj["organizations"].ToString());
                foreach(var organizationObj in organizationArray)
                {
                    var organization = new ZohoOrganization();
                    organization = JsonConvert.DeserializeObject<ZohoOrganization>(organizationObj.ToString());
                    organizationList.Add(organization);
                }
            }
            return organizationList;
        }

        internal static ZohoOrganization getOrganization(HttpResponseMessage responce)
        {
            var organization = new ZohoOrganization();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if (jsonObj.ContainsKey("organization"))
            {
                organization = JsonConvert.DeserializeObject<ZohoOrganization>(jsonObj["organization"].ToString());
            }
            return organization;
        }

        internal static ZohoAddress getOrganizationAddress(HttpResponseMessage responce)
        {
            var address = new ZohoAddress();
            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responce.Content.ReadAsStringAsync().Result);
            if(jsonObj.ContainsKey("organization_address"))
            {
                address = JsonConvert.DeserializeObject<ZohoAddress>(jsonObj["organization_address"].ToString());
            }
            return address;
        }
    }
}
