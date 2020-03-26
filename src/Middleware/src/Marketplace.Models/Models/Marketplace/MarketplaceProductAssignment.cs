using Marketplace.Helpers.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceProductAssignment : ProductAssignment, IMarketplaceObject
    {
        public string ID { get; set; }
    }
}
