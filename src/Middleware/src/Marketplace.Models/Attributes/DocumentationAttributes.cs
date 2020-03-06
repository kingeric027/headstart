﻿using System;
using Marketplace.Helpers.Attributes;

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
    }
}
