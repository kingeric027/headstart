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
using System;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using ImpromptuInterface;
using Cosmonaut;
using Marketplace.Common.Models.Marketplace;

namespace Marketplace.Common.Controllers
{
    public class WebhooksController : BaseController
    {
        private readonly AppSettings _settings;
        private readonly ISendgridService _sendgridService;
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderCloudClient _oc;
        private readonly ResourceHistoryQuery<ProductHistory> _productQuery;
        private readonly ResourceHistoryQuery<PriceScheduleHistory> _priceScheduleQuery;

        private readonly IProductUpdateCommand _productUpdateCommand;

        public WebhooksController(
            AppSettings settings, 
            ISendgridService sendgridService, 
            IOrderCommand orderCommand,
            ResourceHistoryQuery<ProductHistory> productQuery, 
            ResourceHistoryQuery<PriceScheduleHistory> priceScheduleQuery, 
            IOrderCloudClient orderCloud, 
            IProductUpdateCommand productUpdateCommand) : base(settings)
        {
            _settings = settings;
            _sendgridService = sendgridService;
            _orderCommand = orderCommand;
            _productQuery = productQuery;
            _priceScheduleQuery = priceScheduleQuery;
            _productUpdateCommand = productUpdateCommand;
            _oc = orderCloud;
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

        [HttpPost, Route("orderrequiresapproval")]
        [OrderCloudWebhookAuth]
        public async void HandleOrderRequiresApproval([FromBody] MessageNotification<OrderSubmitEventBody> payload)
        {
            await _orderCommand.PatchOrderRequiresApprovalStatus(payload.EventBody.Order.ID);
            await _sendgridService.SendOrderRequiresApprovalEmail(payload);
        }

        [HttpPost, Route("orderdeclined")]
        [OrderCloudWebhookAuth]
        public async void HandleOrderDeclined([FromBody] MarketplaceOrderDeclinePayload payload)
        {
            await _sendgridService.SendOrderDeclinedEmail(payload);
        }

        [HttpPost, Route("newuser")] // TO DO: send email to mp manager
        [OrderCloudWebhookAuth]
        public async void HandleNewUser([FromBody] MessageNotification<PasswordResetEventBody> payload)
        {
            await _sendgridService.SendNewUserEmail(payload);
        }

        [HttpPost, Route("productcreated")]
        [OrderCloudWebhookAuth]
        public async void HandleProductCreation([FromBody] MarketplaceProductCreatePayload payload)
        {
            var update = new ProductHistory()
            {
                Action = ActionType.Create,
                ResourceID = payload.Response.Body.ID,
                Resource = payload.Response.Body,
            };
            await _productQuery.Post(update);
            // to mp manager when a product is created
            //await _sendgridService.SendSingleEmail("noreply@four51.com", "noreply@four51.com", "New Product Created", "<h1>this is a test email for product creation</h1>");
        }

        [HttpPost, Route("productupdated")]
        [OrderCloudWebhookAuth]
        public async void HandleProductUpdate([FromBody] MarketplaceProductUpdatePayload payload)
        {
            Console.WriteLine(payload);
            var update = new ProductHistory()
            {
                Action = ActionType.Update,
                ResourceID = payload.Response.Body.ID,
                Resource = payload.Response.Body,

            };
            await _productQuery.Put(update);

            // querying data and putting into excel spreadsheet
            //await _productUpdateCommand.SendAllProductUpdateEmails();
            // to mp manager when a product is updated
            //await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "Product Updated", "<h1>this is a test email for product update</h1>");
        }

        [HttpPost, Route("priceschedulecreated")]
        [OrderCloudWebhookAuth]
        public async void HandlePriceScheduleCreation([FromBody] WebhookPayloads.PriceSchedules.Create payload)
        {
            var update = new PriceScheduleHistory()
            {
                Action = ActionType.Create,
                ResourceID = payload.Response.Body.ID,
                Resource = payload.Response.Body,
            };
            await _priceScheduleQuery.Post(update);
            // to mp manager when a product is created
            //await _sendgridService.SendSingleEmail("noreply@four51.com", "noreply@four51.com", "New Product Created", "<h1>this is a test email for product creation</h1>");
        }

        [HttpPost, Route("priceScheduleupdated")]
        [OrderCloudWebhookAuth]
        public async void HandlePriceScheduleUpdate([FromBody] WebhookPayloads.PriceSchedules.Patch payload)
        {
            var updatedPriceSchedule = await _oc.PriceSchedules.GetAsync(payload.Request.Body.ID);
            var update = new PriceScheduleHistory()
            {
                Action = ActionType.Update,
                ResourceID = updatedPriceSchedule.ID,
                Resource = updatedPriceSchedule,

            };
            await _priceScheduleQuery.Put(update);

            // querying data and putting into excel spreadsheet
            //  await _productUpdateCommand.SendAllProductUpdateEmails();
            // to mp manager when a product is updated
            //await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "Product Updated", "<h1>this is a test email for product update</h1>");
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