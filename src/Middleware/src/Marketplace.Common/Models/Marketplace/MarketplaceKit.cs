using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Common.Models.Marketplace
{
    [SwaggerModel]
    public class KitProductDocument : Document<KitProduct> { }

    [SwaggerModel]
    public class MarketplaceKitProduct
    {
        public Product Product { get; set; }
        public IList<Asset> Images { get; set; }
        public IList<Asset> Attachments { get; set; }
        public KitProduct ProductAssignments { get; set; }
    }

    [SwaggerModel]
    public class KitProduct
    {
        public IList<ProductInKit> ProductsInKit { get; set; }
    }

    [SwaggerModel]
    public class ProductInKit
    {
        public string ID { get; set; }
        public bool Required { get; set; }
        public int MinQty { get; set; }
        public int MaxQty { get; set; }
    }

}
