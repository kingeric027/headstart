﻿using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models
{
    public class MarketplaceAddressBuyer : Address<BuyerAddressXP>, IMarketplaceObject
    {
    }
    public class MarketplaceAddressMeBuyer : BuyerAddress<BuyerAddressXP>, IMarketplaceObject
    {
    }

    public class BuyerAddressXP { 
        public List<DestinationAddressAccessorial> Accessorials { get; set; }
    }

    public enum DestinationAddressAccessorial
    {
        DestinationInsideDelivery = 3,
        DestinationLiftGate = 4,
        LimitedAccessDelivery = 9,
        ResidentialDelivery = 15,
    }
}
