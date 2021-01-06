using Marketplace.Common.Models.Marketplace;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    public class PaymentUpdateRequest
    {
        public List<HSPayment> Payments { get; set; }
    }
}
