using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderPatchPayload : WebhookPayload<MarketplaceOrder, MarketplaceOrder, WebhookPayloads.Orders.Patch.PatchRouteParams, dynamic>
    {
        public OrderDirection Direction { get; set; }
        public string OrderID { get; set; }
    }
}
