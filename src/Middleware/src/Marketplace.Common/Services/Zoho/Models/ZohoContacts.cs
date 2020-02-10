using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoContacts.
    /// </summary>
    public class ZohoContacts
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
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>The contacts.</value>
        public List<ZohoContact> contacts { get; set; }
    }
}
