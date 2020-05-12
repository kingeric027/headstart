using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderApprovePayload : WebhookPayloads.Orders.Approve<dynamic, OrderApprovalInfo, MarketplaceOrder> { }
}
