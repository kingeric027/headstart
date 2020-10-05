using Marketplace.Models;
using ordercloud.integrations.cms;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Common.Models.Marketplace
{
    [SwaggerModel]
    public class KitProductDocument : Document<KitProduct> { }

    [SwaggerModel]
    public class MarketplaceMeKitProduct
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public MarketplaceMeProduct Product { get; set; }
        public IList<Asset> Images { get; set; }
        public IList<Asset> Attachments { get; set; }
        public MeKitProduct ProductAssignments { get; set; }
    }

    [SwaggerModel]
    public class MarketplaceKitProduct
    {
        public string ID { get; set; }
        public string Name { get; set; }
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
    public class MeKitProduct
    {
        public IList<MeProductInKit> ProductsInKit { get; set; }
    }

    [SwaggerModel]
    public class ProductInKit
    {
        public string ID { get; set; }
        public int? MinQty { get; set; }
        public int? MaxQty { get; set; }
        public bool Static { get; set; }
        public string SpecCombo { get; set; }
        public IList<Variant> Variants { get; set; }
        public IList<Spec> Specs { get; set; }
        public MarketplaceProduct Product { get; set; }
    }

    [SwaggerModel]
    public class MeProductInKit
    {
        public string ID { get; set; }
        public int? MinQty { get; set; }
        public int? MaxQty { get; set; }
        public bool Static { get; set; }
        public string SpecCombo { get; set; }
        public IList<Variant> Variants { get; set; }
        public IList<Spec> Specs { get; set; }
        public MarketplaceMeProduct Product { get; set; }
    }

}
