﻿using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplacePriceSchedule : PriceSchedule<PriceScheduleXp>, IMarketplaceObject
    {
        
    }

    [SwaggerModel]
    public class PriceScheduleXp
    {
    }
}
