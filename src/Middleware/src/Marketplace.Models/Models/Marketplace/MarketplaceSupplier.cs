using System.Collections.Generic;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Extended;
using Marketplace.Models.Marketplace.Extended;
using OrderCloud.SDK;

namespace Marketplace.Models.Models.Marketplace
{
    [SwaggerModel]
    public class MarketplaceSupplier : Supplier<SupplierXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class SupplierXp
    {
        public string Description { get; set; }
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public Contact SupportContact { get; set; }
        public bool SyncFreightPop { get; set; }
        public string ApiClientID { get; set; }
    }
}
