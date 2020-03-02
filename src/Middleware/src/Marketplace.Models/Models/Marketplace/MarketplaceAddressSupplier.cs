using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models
{
    public class MarketplaceAddressSupplier : Address<SupplierAddressXP>, IMarketplaceObject
    {
    }

    public class SupplierAddressXP
    {
        public List<OriginAddressAccessorial> Accessorials { get; set; }
    }

    public enum OriginAddressAccessorial
    {
        LimitedAccessPickup = 10,
        OriginExhibition = 11,
        OriginInsidePickup = 12,
        OriginLiftGate = 13,
        ResidentialPickup = 16
    }
}
