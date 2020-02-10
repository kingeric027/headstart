using System.Collections.Generic;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceProductFacet : ProductFacet<ProductFacetXp>, IMarketplaceObject
    {
        
    }

    public class ProductFacetXp
    {
        public IEnumerable<string> Options { get; set; }
        public string ParentID { get; set; }
    }
}
