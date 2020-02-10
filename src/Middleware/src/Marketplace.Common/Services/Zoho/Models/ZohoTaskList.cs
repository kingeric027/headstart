using System.Collections.Generic;

namespace Marketplace.Common.Services.Zoho.Models
{
    /// <summary>
    /// Used to define the model object of TasksList.
    /// </summary>
    public class ZohoTaskList:List<ZohoProjectTask>
    {
        /// <summary>
        /// Gets or sets the ZohoPageContext.
        /// </summary>
        /// <value>The ZohoPageContext.</value>
        public ZohoPageContext ZohoPageContext { get; set; }
    }
}
