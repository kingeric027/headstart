using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ListOfBaseCurrencyAdjustments.
    /// </summary>
    public class ZohoBaseCurrencyAdjustmentsList:List<ZohoBaseCurrencyAdjustment>
    {
        
        /// <summary>
        /// Gets or sets the ZohoPageContext.
        /// </summary>
        /// <value>The ZohoPageContext.</value>
        public ZohoPageContext ZohoPageContext { get; set; }
    }
}
