using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class Customfields.
    /// </summary>
    public class ZohoCustomFieldList
    {
        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        /// <value>The invoice.</value>
        public List<ZohoCustomField> invoice { get; set; }
        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        /// <value>The contact.</value>
        public List<ZohoCustomField> contact { get; set; }
        /// <summary>
        /// Gets or sets the estimate.
        /// </summary>
        /// <value>The estimate.</value>
        public List<ZohoCustomField> estimate { get; set; }
    }
}
