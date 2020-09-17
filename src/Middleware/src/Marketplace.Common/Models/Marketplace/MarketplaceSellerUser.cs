using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class MarketplaceSellerUser : User<SellerUserXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class SellerUserXp
    {
        public bool OrderEmails { get; set; } = false;
        public List<string> AddtlRcpts { get; set; } = new List<string>();
    }
}
