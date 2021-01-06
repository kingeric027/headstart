using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.library;

namespace Marketplace.Common.Models.Marketplace
{
    [SwaggerModel]
    public class HSLineItemOrder
    {
        public HSOrder HSOrder { get; set; }
        public HSLineItem HSLineItem { get; set; }
    }
}
