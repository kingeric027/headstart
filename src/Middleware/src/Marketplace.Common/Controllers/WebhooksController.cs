using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using System;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Controllers
{
    public class WebhooksController : BaseController
    {

        private readonly AppSettings _settings;
        private readonly ISendgridService _sendgridService;
        private readonly IOrderCloudClient _oc;

        public WebhooksController(AppSettings settings, ISendgridService sendgridService) : base(settings)
        {
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ClientId = "2234C6E1-8FA5-41A2-8A7F-A560C6BA44D8",
                ClientSecret = "z08ibzgsb337ln8EzJx5efI1VKxqdqeBW0IB7p1SJaygloJ4J9uZOtPu1Aql",
                Roles = new[] { ApiRole.FullAccess }
            });
            _settings = settings;
            _sendgridService = sendgridService;
        }

        // USING AN OC MESSAGE SENDER - NOT WEBHOOK
        [HttpPost, Route("passwordreset")]
        public async void HandleBuyerPasswordReset([FromBody] MessageNotification payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", payload.Recipient.Email, "Password Reset", "<h1>this is a test email for password reset</h1>");
        }

        [HttpPost, Route("ordersubmit")]
        public async void HandleOrderSubmit([FromBody] WebhookPayloads.Orders.Submit payload)
        {
            List<string> SupplierList = new List<string>();
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, payload.Response.Body.ID);

                foreach (var item in lineItems.Items)
                {
                    SupplierList.Add(item.SupplierID);
                }
                var uniqueSupplierList = SupplierList.Distinct().ToList();
                foreach (var supplier in uniqueSupplierList)
                {
                    var supplierInfo = await _oc.Suppliers.GetAsync(supplier);
                    await _sendgridService.SendSingleEmail("noreply@four51.com", supplierInfo.xp.Contacts[0].Email, "Order Confirmation", "<h1>this is a test email for order submit</h1>"); // to supplier receiving order
                }

            await _sendgridService.SendSingleEmail("noreply@four51.com", payload.Response.Body.FromUser.Email, "Order Confirmation", "<h1>this is a test email for order submit</h1>"); // to buyer placing order
        }

        [HttpPost, Route("orderrequiresapproval")] // TO DO: TEST & FIND PROPER PAYLOAD
        public async void HandleOrderRequiresApproval(JObject payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "order sent for approval test", "<h1>this is a test email for order sent for approval</h1>"); // to buyer whose order needs approval
            await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "order requires approval test", "<h1>this is a test email for order requires approval</h1>"); // to approver who needs to approve order
        }

        [HttpPost, Route("orderapproved")] // TO DO: TEST
        public async void HandleOrderApproved([FromBody] WebhookPayloads.Orders.Approve payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Approved", "<h1>this is a test email for order approved</h1>");
        }

        [HttpPost, Route("orderdeclined")] // TO DO: TEST
        public async void HandleOrderDeclined([FromBody] WebhookPayloads.Orders.Decline payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Declined", "<h1>this is a test email for order declined</h1>");
        }

        [HttpPost, Route("ordershipped")] // TO DO: TEST
        public async void HandleOrderShipped([FromBody] WebhookPayloads.Orders.Ship payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Shipped", "<h1>this is a test email for order shipped</h1>");
        }

        [HttpPost, Route("orderdelivered")] // TO DO: TEST & FIND PROPER PAYLOAD
        public async void HandleOrderDelivered(JObject payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Delivered", "<h1>this is a test email for order delivered</h1>");
        }

        [HttpPost, Route("ordercancelled")] // TO DO: TEST 
        public async void HandleOrderCancelled([FromBody] WebhookPayloads.Orders.Cancel payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Cancelled", "<h1>this is a test email for order cancelled</h1>");
        }

        [HttpPost, Route("orderrefund")] // TO DO: TEST & FIND PROPER PAYLOAD
        public async void HandleOrderRefund(JObject payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Refunded", "<h1>this is a test email for order refunded</h1>");
        }

        [HttpPost, Route("orderupdated")] // TO DO: TEST 
        public async void HandleOrderUpdated([FromBody] WebhookPayloads.Orders.Patch payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", "scasey@four51.com", "Order Updated", "<h1>this is a test email for order update</h1>");
        }

        [HttpPost, Route("newuser")]
        public async void HandleNewUser([FromBody] WebhookPayloads.Users.Create payload)
        {
            await _sendgridService.SendSingleEmail("noreply@four51.com", payload.Response.Body.Email, "New Buyer User", "<h1>this is a test email for buyer user creation</h1>"); // to buyer - welcome email
            await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "New Buyer User", "<h1>this is a test email for buyer user creation</h1>"); // to admin - new user registered on buyer site
        }

        [HttpPost, Route("productcreated")]
        public async void HandleProductCreation([FromBody] WebhookPayloads.Products.Create payload)
        {
            // to mp manager when a product is created
            await _sendgridService.SendSingleEmail("noreply@four51.com", "noreply@four51.com", "New Product Created", "<h1>this is a test email for product creation</h1>");
        }

        [HttpPost, Route("productupdate")]
        public async void HandleProductUpdate([FromBody] WebhookPayloads.Products.Patch payload)
        {
            // to mp manager when a product is updated
            await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "Product Updated", "<h1>this is a test email for product update</h1>");
        }

        [HttpPost, Route("supplierupdated")]
        public async void HandleSupplierUpdate([FromBody] WebhookPayloads.Suppliers.Patch payload)
        {
            // to mp manager when a supplier is updated
            await _sendgridService.SendSingleEmail("noreply@four51.com", "to", "Supplier Updated", "<h1>this is a test email for supplier update</h1>");
        }
    }
}