using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using System;

namespace Marketplace.Common.Controllers
{
    public class WebhooksController : BaseController
    {

        private readonly AppSettings _settings;
        private readonly ISendgridService _sendgridService;

        public WebhooksController(AppSettings settings, ISendgridService sendgridService) : base(settings)
        {
            _settings = settings;
            _sendgridService = sendgridService;
        }

        // BUYER EMAILS

        [HttpPost, Route("ordersubmit")] // TESTED - WORKS
        public async void HandleOrderSubmit([FromBody] WebhookPayloads.Orders.Submit payload)
        {
            Console.WriteLine("order submit received", payload);
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "Order Confirmation", "<h1>this is a test email for order submit</h1>");
        }

        [HttpPost, Route("ordersentforapproval")] // TO DO: TEST & FIND PROPER PAYLOAD
        public async void HandleOrderSentForApproval([FromBody] JObject payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "order sent for approval test", "<h1>this is a test email for order sent for approval</h1>");
        }

        [HttpPost, Route("orderrequiresapproval")] // TO DO: TEST & FIND PROPER PAYLOAD
        public async void HandleOrderRequiresApproval([FromBody] JObject payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "order requires approval test", "<h1>this is a test email for order requires approval</h1>");
        }

        [HttpPost, Route("orderapproved")] // TO DO: TEST & FIND PROPER PAYLOAD
        public async void HandleOrderApproved([FromBody] WebhookPayloads.Orders.Approve payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "Order Approved", "<h1>this is a test email for order approved</h1>");
        }

        [HttpPost, Route("orderdeclined")] // TO DO: TEST
        public async void HandleOrderDeclined([FromBody] WebhookPayloads.Orders.Decline payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "Order Declined", "<h1>this is a test email for order declined</h1>");
        }

        [HttpPost, Route("ordershipped")] // TO DO: TEST
        public async void HandleOrderShipped([FromBody] WebhookPayloads.Orders.Ship payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "Order Shipped", "<h1>this is a test email for order shipped</h1>");
        }

        [HttpPost, Route("orderdelivered")] // TO DO: TEST & FIND PROPER PAYLOAD
        public async void HandleOrderDelivered([FromBody] JObject payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "Order Delivered", "<h1>this is a test email for order delivered</h1>");
        }

        [HttpPost, Route("newbuyeruser")] // TESTED - WORKS (STOPPED WORKING AFTER REFACTOR)
        public async void HandleNewBuyerUser([FromBody] WebhookPayloads.Users.Create payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "New Buyer User", "<h1>this is a test email for buyer user creation</h1>");
        }

        // USING AN OC MESSAGE SENDER - NOT SENDGRID
        [HttpPost, Route("passwordreset")]
        public void HandleBuyerPasswordReset([FromBody] MessageNotification payload)
        {
        }

        // MP MANAGER EMAILS

        [HttpPost, Route("productcreated")] // TESTED - WORKS (STOPPED WORKING AFTER REFACTOR)
        public async void HandleProductCreation([FromBody] JObject payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "New Product Created", "<h1>this is a test email for product creation</h1>");
        }

        [HttpPost, Route("productupdate")] // TO DO: TEST
        public async void HandleProductUpdate([FromBody] JObject payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "Product Updated", "<h1>this is a test email for product update</h1>");
        }

        [HttpPost, Route("supplierupdated")] // TO DO: TEST
        public async void HandleSupplierUpdate([FromBody] JObject payload)
        {
            await _sendgridService.SendSingleEmail("scasey@four51.com", "scasey@four51.com", "Supplier Updated", "<h1>this is a test email for supplier update</h1>");
        }
    }
}