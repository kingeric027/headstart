using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoContactList.
    /// </summary>
    public class ZohoContactList:List<ZohoContact>
    {
        /// <summary>
        /// Gets or sets the ZohoPageContext.
        /// </summary>
        /// <value>The ZohoPageContext.</value>
        public ZohoPageContext ZohoPageContext { get; set; }
    }
}
