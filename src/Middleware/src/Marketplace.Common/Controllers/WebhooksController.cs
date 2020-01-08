using Marketplace.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
    public class WebhooksController : BaseController
    {
        public WebhooksController(AppSettings settings) : base(settings)
        {
        }

        [HttpPost, Route("ordersubmit")]
        public void HandleOrderSubmitWebhook([FromBody] WebhookPayloads.Orders.Submit payload)
        {
            Console.WriteLine("order submit webhook received");
        }
        [HttpPost, Route("passwordreset")]
        public void HandleResetPassword([FromBody] MessageNotification payload)
        {
            Console.WriteLine("password reset message received");
        }
    }
}