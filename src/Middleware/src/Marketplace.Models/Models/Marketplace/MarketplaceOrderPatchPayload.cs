using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderPatchPayload : WebhookPayloads.Orders.Patch<dynamic, MarketplaceOrder> { }
}
