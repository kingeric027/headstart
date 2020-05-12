using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderDeclinePayload : WebhookPayloads.Orders.Decline<dynamic, OrderApprovalInfo, MarketplaceOrder> { }
}
