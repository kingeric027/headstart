using Headstart.Models;
using Headstart.Models.Models.Marketplace;
using ordercloud.integrations.library;

namespace Headstart.Common.Models.Marketplace
{
    [SwaggerModel]
    public class HSLineItemOrder
    {
        public HSOrder HSOrder { get; set; }
        public HSLineItem HSLineItem { get; set; }
    }
}
