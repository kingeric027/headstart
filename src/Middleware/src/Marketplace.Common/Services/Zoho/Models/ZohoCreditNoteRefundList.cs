using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoCreditNoteRefundList.
    /// </summary>
    public class ZohoCreditNoteRefundList:List<ZohoCreditNote>
    {
        /// <summary>
        /// Gets or sets the ZohoPageContext.
        /// </summary>
        /// <value>The ZohoPageContext.</value>
        public ZohoPageContext ZohoPageContext { get; set; }
    }
}
