namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoFromEmail.
    /// </summary>
    public class ZohoFromEmail
    {
        /// <summary>
        /// Gets or sets the user_name.
        /// </summary>
        /// <value>The user_name.</value>
        public string user_name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoFromEmail" /> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool selected { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string email { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoFromEmail"/> is is_org_email_id.
        /// </summary>
        /// <value><c>true</c> if is_org_email_id; otherwise, <c>false</c>.</value>
        public bool is_org_email_id { get; set; }
    }
}
