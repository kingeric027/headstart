using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderApprovePayload : WebhookPayloads.Orders.Approve<dynamic, OrderApprovalInfo, MarketplaceOrder> { }
}
