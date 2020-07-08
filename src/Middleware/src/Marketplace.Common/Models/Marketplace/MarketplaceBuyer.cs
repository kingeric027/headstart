using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class SuperMarketplaceBuyer
    {
        public MarketplaceBuyer Buyer { get; set; }
        public BuyerMarkup Markup { get; set; }
    }

    [SwaggerModel]
    public class MarketplaceBuyer : Buyer<BuyerXp>, IMarketplaceObject
    {
        
    }

    // just int for now, but leaving the door open for future configurations on how this markup functions
    public class BuyerMarkup
    {
        public int Percent { get; set; }
    }

    [SwaggerModel]
    public class BuyerXp
    {
        // temporary field while waiting on content docs
        public int MarkupPercent { get; set; }
    }
}
