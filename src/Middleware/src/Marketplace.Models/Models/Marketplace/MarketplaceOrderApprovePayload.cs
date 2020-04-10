using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderApprovePayload : WebhookPayload<OrderApprovalInfo, MarketplaceOrder, WebhookPayloads.Orders.Approve.ApproveRouteParams, dynamic>
    {
        public OrderDirection Direction { get; set; }
        public string OrderID { get; set; }
    }
}
