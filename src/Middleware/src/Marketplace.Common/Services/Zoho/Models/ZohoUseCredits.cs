using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoUseCredits.
    /// </summary>
    public class ZohoUseCredits
    {
        /// <summary>
        /// Gets or sets the invoice_payments.
        /// </summary>
        /// <value>The invoice_payments.</value>
        public List<ZohoPayment> invoice_payments { get; set; }
        /// <summary>
        /// Gets or sets the apply_creditnotes.
        /// </summary>
        /// <value>The apply_creditnotes.</value>
        public List<ZohoCreditNote> apply_creditnotes { get; set; }
        /// <summary>
        /// Gets or sets the bill_payments.
        /// </summary>
        /// <value>The bill_payments.</value>
        public List<ZohoPayment> bill_payments { get; set; }
    }
}
