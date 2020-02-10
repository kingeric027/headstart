using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoEmailNotification.
    /// </summary>
    public class ZohoEmailNotification
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZohoEmailNotification" /> is send_from_org_email_id.
        /// </summary>
        /// <value><c>true</c> if send_from_org_email_id; otherwise, <c>false</c>.</value>
        public bool send_from_org_email_id { get; set; }
        /// <summary>
        /// Gets or sets the to_mail_ids.
        /// </summary>
        /// <value>The to_mail_ids.</value>
        public List<string> to_mail_ids { get; set; }
        /// <summary>
        /// Gets or sets the cc_mail_ids.
        /// </summary>
        /// <value>The cc_mail_ids.</value>
        public List<string> cc_mail_ids { get; set; }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string subject { get; set; }
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public string body { get; set; }
    }
}
