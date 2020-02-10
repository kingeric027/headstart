using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoPaymentOptions.
    /// </summary>
    public class ZohoPaymentOptions
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public int code { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public int message { get; set; }
        /// <summary>
        /// Gets or sets the payment_gateways.
        /// </summary>
        /// <value>The payment_gateways.</value>
        public List<ZohoPaymentGateway> payment_gateways { get; set; }
    }
}
