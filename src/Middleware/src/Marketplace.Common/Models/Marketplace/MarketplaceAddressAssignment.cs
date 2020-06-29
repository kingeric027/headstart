﻿using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceAddressAssignment : AddressAssignment, IMarketplaceObject
    {
        public string ID { get; set; }
    }
}