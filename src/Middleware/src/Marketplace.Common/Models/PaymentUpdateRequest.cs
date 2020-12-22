using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    public class PaymentUpdateRequest
    {
        public List<Payment> Payments { get; set; }
    }
}
