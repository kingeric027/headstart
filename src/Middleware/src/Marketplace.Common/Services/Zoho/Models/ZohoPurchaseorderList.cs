using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoPurchaseorderList.
    /// </summary>
    public class ZohoPurchaseorderList:List<ZohoPurchaseOrder>
    {
        /// <summary>
        /// Gets or sets the ZohoPageContext.
        /// </summary>
        /// <value>The ZohoPageContext.</value>
        public ZohoPageContext ZohoPageContext { get; set; }
    }
}
