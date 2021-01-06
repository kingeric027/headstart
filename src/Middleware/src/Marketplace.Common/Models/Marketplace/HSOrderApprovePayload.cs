using OrderCloud.SDK;

namespace Marketplace.Models
{
    public class HSOrderApprovePayload : WebhookPayloads.Orders.Approve<dynamic, OrderApprovalInfo, HSOrder> { }
}
