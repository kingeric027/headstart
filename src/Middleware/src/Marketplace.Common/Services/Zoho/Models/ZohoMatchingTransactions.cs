using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoMatchingTransactions.
    /// </summary>
    public class ZohoMatchingTransactions:List<ZohoTransaction>
    {
        /// <summary>
        /// Gets or sets the ZohoPageContext.
        /// </summary>
        /// <value>The ZohoPageContext.</value>
        public ZohoPageContext ZohoPageContext { get; set; }
        /// <summary>
        /// Gets or sets the ZohoInstrumentation.
        /// </summary>
        /// <value>The ZohoInstrumentation.</value>
        public ZohoInstrumentation ZohoInstrumentation { get; set; }
    }
}
