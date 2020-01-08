using Marketplace.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
    public class WebhooksController : BaseController
    { 

        private readonly AppSettings _settings;
        
        public WebhooksController(AppSettings settings) : base(settings)
        {
            _settings = settings;
        }

        [HttpPost, Route("ordersubmit")]
        public void HandleOrderSubmitWebhook([FromBody] WebhookPayloads.Orders.Submit payload)
        {
            async Task Execute()
            {
                var apiKey = _settings.SendgridApiKey;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("scasey@four51.com", "Sydney");
                var subject = "order submit test";
                var to = new EmailAddress("scasey@four51.com", "Sydney");
                var plainTextContent = "order submit email test";
                var htmlContent = "<strong style='color:pink';>test</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            Console.WriteLine("order submit webhook received");
            Execute().Wait();
        }
        [HttpPost, Route("passwordreset")]
        public void HandleResetPassword([FromBody] MessageNotification payload)
        {
            Console.WriteLine("password reset message received");
        }
    }
}