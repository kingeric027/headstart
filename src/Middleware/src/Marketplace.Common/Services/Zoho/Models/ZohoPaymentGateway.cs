namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoPaymentGateway.
    /// </summary>
    public class ZohoPaymentGateway
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoPaymentGateway" /> is configured.
        /// </summary>
        /// <value><c>true</c> if configured; otherwise, <c>false</c>.</value>
        public bool configured { get; set; }
        /// <summary>
        /// Gets or sets the additional_field1.
        /// </summary>
        /// <value>The additional_field1.</value>
        public string additional_field1 { get; set; }
        /// <summary>
        /// Gets or sets the gateway_name.
        /// </summary>
        /// <value>The gateway_name.</value>
        public string gateway_name { get; set; }
    }
}
