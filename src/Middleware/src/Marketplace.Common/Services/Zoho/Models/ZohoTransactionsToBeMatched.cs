using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoTransactionsToBeMatched.
    /// </summary>
    public class ZohoTransactionsToBeMatched
    {
        /// <summary>
        /// Gets or sets the transactions_to_be_matched.
        /// </summary>
        /// <value>The transactions_to_be_matched.</value>
        public List<ZohoTransaction> transactions_to_be_matched { get; set; }
    }
}
