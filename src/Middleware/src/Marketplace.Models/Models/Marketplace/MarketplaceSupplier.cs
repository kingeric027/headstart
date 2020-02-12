using System.Collections.Generic;
using Marketplace.Models.Extended;
using Marketplace.Models.Marketplace.Extended;
using OrderCloud.SDK;

namespace Marketplace.Models.Models.Marketplace
{
    public class MarketplaceSupplier : Supplier<SupplierXp>
    {
    }

    public class SupplierXp
    {
        public string Description { get; set; }
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public Contact SupportContact { get; set; }
    }

    
}
