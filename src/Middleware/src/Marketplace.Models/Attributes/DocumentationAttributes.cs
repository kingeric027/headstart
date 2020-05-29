using System;
using ordercloud.integrations.library;

namespace Marketplace.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MarketplaceSection : DocSection
    {
        public MarketplaceSection()
        {
            
        }
        // <summary>
        /// Use on controllers to indicate that they belong in the Orchestration section of the API reference docs.
        /// </summary>
        public class OrchestrationAttribute : DocSection { }
        /// <summary>
        /// Use on controllers to indicate that they belong in the Marketplace section of the API reference docs.
        /// </summary>
        public class MarketplaceAttribute : DocSection { }

        public class IntegrationAttribute : DocSection { }

		/// <summary>
		/// Use on controllers to indicate that they belong in the Content section of the API reference docs.
		/// </summary>
		public class ContentAttribute : DocSection { }
	}
}
