using Marketplace.Common.Commands;
using Marketplace.Common.Services;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using Marketplace.Common.Helpers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers
{
    public class WebhooksController : BaseController
    {
        private readonly AppSettings _settings;
        private readonly ISendgridService _sendgridService;
        private readonly IOrderCommand _orderCommand;

        public WebhooksController(AppSettings settings, ISendgridService sendgridService, IOrderCommand orderCommand) : base(settings)
        {
            _settings = settings;
            _sendgridService = sendgridService;
            _orderCommand = orderCommand;
        }

        // USING AN OC MESSAGE SENDER - NOT WEBHOOK
        [HttpPost, Route("passwordreset")]
        [OrderCloudWebhookAuth]
        public async void HandleBuyerPasswordReset([FromBody] MessageNotification<PasswordResetEventBody> payload)
        {
            await _sendgridService.SendPasswordResetEmail(payload);
        }

        [HttpPost, Route("ordersubmittedforapproval")]
        [OrderCloudWebhookAuth]
        public async void HandleOrderSubmittedForApproval([FromBody] MessageNotification<OrderSubmitEventBody> payload)
        {
            await _sendgridService.SendOrderSubmittedForApprovalEmail(payload);
        }

        [HttpPost, Route("ordersubmit")]
        [OrderCloudWebhookAuth]
        public async Task<OrderSubmitResponse> HandleOrderSubmit([FromBody] OrderCalculatePayload<MarketplaceOrderWorksheet> payload)
        {
            var response = await _orderCommand.HandleBuyerOrderSubmit(payload.OrderWorksheet);
            return response;
        }

        [HttpPost, Route("orderrequiresapproval")]
        [OrderCloudWebhookAuth]
        public async void HandleOrderRequiresApproval([FromBody] MessageNotification<OrderSubmitEventBody> payload)
        {
            await _sendgridService.SendOrderRequiresApprovalEmail(payload);
        }

        [HttpPost, Route("orderapproved")]
        [OrderCloudWebhookAuth]
        public async void HandleOrderApproved([FromBody] MarketplaceOrderApprovePayload payload)
        {
            await _sendgridService.SendOrderApprovedEmail(payload);
        }

        [HttpPost, Route("orderdeclined")]
        [OrderCloudWebhookAuth]
        public async void HandleOrderDeclined([FromBody] MarketplaceOrderDeclinePayload payload)
        {
            await _sendgridService.SendOrderDeclinedEmail(payload);
        }

        [HttpPost, Route("ordershipped")] // TO DO: TEST
        [OrderCloudWebhookAuth]
        public async void HandleOrderShipped([FromBody] WebhookPayloads.Orders.Ship payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Shipped", "<h1>this is a test email for order shipped</h1>");
        }

        [HttpPost, Route("orderdelivered")] // TO DO: TEST & FIND PROPER PAYLOAD, ADD TO ENV SEED PROCESS
        [OrderCloudWebhookAuth]
        public async void HandleOrderDelivered(JObject payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Delivered", "<h1>this is a test email for order delivered</h1>");
        }

        [HttpPost, Route("ordercancelled")] // TO DO: TEST 
        [OrderCloudWebhookAuth]
        public async void HandleOrderCancelled([FromBody] WebhookPayloads.Orders.Cancel payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Cancelled", "<h1>this is a test email for order cancelled</h1>");
        }

        [HttpPost, Route("orderrefund")] // TO DO: TEST & FIND PROPER PAYLOAD, ADD TO ENV SEED PROCESS
        [OrderCloudWebhookAuth]
        public async void HandleOrderRefund(JObject payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Refunded", "<h1>this is a test email for order refunded</h1>");
        }

        [HttpPost, Route("orderupdated")]
        [OrderCloudWebhookAuth]
        public async void HandleOrderUpdated([FromBody] MarketplaceOrderPatchPayload payload)
        {
            await _sendgridService.SendOrderUpdatedEmail(payload);
        }

        [HttpPost, Route("newuser")] // TO DO: send email to mp manager
        [OrderCloudWebhookAuth]
        public async void HandleNewUser([FromBody] WebhookPayloads.Users.Create payload)
        {
            await _sendgridService.SendNewUserEmail(payload);
        }

        [HttpPost, Route("productcreated")]
        [OrderCloudWebhookAuth]
        public async void HandleProductCreation([FromBody] WebhookPayloads.Products.Create payload)
        {
            // to mp manager when a product is created
            await _sendgridService.SendSingleEmail("noreply@four51.com", "noreply@four51.com", "New Product Created", "<h1>this is a test email for product creation</h1>");
        }

        [HttpPost, Route("productupdate")]
        [OrderCloudWebhookAuth]
        public async void HandleProductUpdate([FromBody] WebhookPayloads.Products.Patch payload)
        {
            // to mp manager when a product is updated
            await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "Product Updated", "<h1>this is a test email for product update</h1>");
        }

        [HttpPost, Route("supplierupdated")]
        [OrderCloudWebhookAuth]
        public async void HandleSupplierUpdate([FromBody] WebhookPayloads.Suppliers.Patch payload)
        {
            // to mp manager when a supplier is updated
            await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "Supplier Updated", "<h1>this is a test email for supplier update</h1>");
        }
    }
}