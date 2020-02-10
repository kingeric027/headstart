using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceSpecOption : SpecOption<SpecOptionXp>, IMarketplaceObject
    {
    }

    public class SpecOptionXp
    {
        public string Description { get; set; }
        public string SpecID { get; set; }
    }
}
