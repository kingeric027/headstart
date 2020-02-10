using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of ZohoUsersToAssign.
    /// </summary>
    public class ZohoUsersToAssign
    {
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public List<ZohoUser> users { get; set; }
    }
}
