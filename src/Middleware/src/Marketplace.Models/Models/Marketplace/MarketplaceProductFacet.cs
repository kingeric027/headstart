using System.Collections.Generic;
using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceProductFacet : ProductFacet<ProductFacetXp>, IMarketplaceObject
    {
        
    }

    [SwaggerModel]
    public class ProductFacetXp
    {
        public IEnumerable<string> Options { get; set; }
        public string ParentID { get; set; }
    }
}
