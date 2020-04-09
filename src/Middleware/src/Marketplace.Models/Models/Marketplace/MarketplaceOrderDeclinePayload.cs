using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class MarketplaceOrderDeclinePayload : WebhookPayload<OrderApprovalInfo, MarketplaceOrder, WebhookPayloads.Orders.Decline.DeclineRouteParams, dynamic>
    {
        public OrderDirection Direction { get; set; }
        public string OrderID { get; set; }
    }
}
