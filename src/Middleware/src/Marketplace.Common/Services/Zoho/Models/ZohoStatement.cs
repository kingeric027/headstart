using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoStatement.
    /// </summary>
    public class ZohoStatement
    {
        /// <summary>
        /// Gets or sets the statement_id.
        /// </summary>
        /// <value>The statement_id.</value>
        public string statement_id { get; set; }
        /// <summary>
        /// Gets or sets the from_date.
        /// </summary>
        /// <value>The from_date.</value>
        public string from_date { get; set; }
        /// <summary>
        /// Gets or sets the to_date.
        /// </summary>
        /// <value>The to_date.</value>
        public string to_date { get; set; }
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public string source { get; set; }
        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        /// <value>The transactions.</value>
        public List<ZohoTransaction> transactions { get; set; }
    }
}
