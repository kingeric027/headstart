using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Class ZohoTaxGroup.
    /// </summary>
    public class ZohoTaxGroup
    {
        /// <summary>
        /// Gets or sets the tax_group_id.
        /// </summary>
        /// <value>The tax_group_id.</value>
        public string tax_group_id { get; set; }
        /// <summary>
        /// Gets or sets the tax_group_name.
        /// </summary>
        /// <value>The tax_group_name.</value>
        public string tax_group_name { get; set; }
        /// <summary>
        /// Gets or sets the tax_group_percentage.
        /// </summary>
        /// <value>The tax_group_percentage.</value>
        public double tax_group_percentage { get; set; }
        /// <summary>
        /// Gets or sets the taxes.
        /// </summary>
        /// <value>The taxes.</value>
        public List<ZohoTax> taxes { get; set; }
        
    }
}
