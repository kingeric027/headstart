using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoApplyToInvoices.
    /// </summary>
    public class ZohoApplyToInvoices
    {
        /// <summary>
        /// Gets or sets the invoices.
        /// </summary>
        /// <value>The invoices.</value>
        public List<ZohoCreditedInvoice> invoices { get; set; }
    }
}
