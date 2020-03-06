using Marketplace.Models.Models.Marketplace;
using System.Collections.Generic;
using Marketplace.Helpers.Attributes;

namespace Marketplace.Models.Misc
{
    [DocIgnore]
    public class EnvironmentSeed
    {
        public List<MarketplaceSupplier> Suppliers { get; set; }
    }
}
