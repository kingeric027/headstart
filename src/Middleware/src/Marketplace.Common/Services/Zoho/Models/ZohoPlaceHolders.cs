using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoPlaceHolders.
    /// </summary>
    public class ZohoPlaceHolders
    {
        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        /// <value>The invoice.</value>
        public List<ZohoInvoice> Invoice { get; set; }
        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>The customer.</value>
        public List<ZohoCustomer> Customer { get; set; }
        /// <summary>
        /// Gets or sets the ZohoOrganization.
        /// </summary>
        /// <value>The ZohoOrganization.</value>
        public List<ZohoOrganization> Organization { get; set; }
    }
}
