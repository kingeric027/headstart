using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderPatchPayload : WebhookPayloads.Orders.Patch<dynamic, MarketplaceOrder> { }
}
