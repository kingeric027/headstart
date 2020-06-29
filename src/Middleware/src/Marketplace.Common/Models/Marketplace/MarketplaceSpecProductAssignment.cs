﻿using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceSpecProductAssignment : SpecProductAssignment, IMarketplaceObject
    {
        public string ID { get; set; }
    }
}