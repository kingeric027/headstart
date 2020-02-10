using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoCommentList.
    /// </summary>
    public class ZohoCommentList:List<ZohoComment>
    {
        /// <summary>
        /// Gets or sets the ZohoPageContext.
        /// </summary>
        /// <value>The ZohoPageContext.</value>
        public ZohoPageContext ZohoPageContext { get; set; }
    }
}
