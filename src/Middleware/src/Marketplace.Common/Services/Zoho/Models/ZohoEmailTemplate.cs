namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of Emailtemplate.
    /// </summary>
    public class ZohoEmailTemplate
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoEmailTemplate" /> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool selected { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the email_template_id.
        /// </summary>
        /// <value>The email_template_id.</value>
        public string email_template_id { get; set; }
    }
}
