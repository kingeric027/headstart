using Avalara.AvaTax.RestClient;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models.Marketplace
{
    public class HSOrderCalculateResponse : OrderCalculateResponse<OrderCalculateResponseXp>
    {
    }

    public class OrderCalculateResponseXp
    {
        public TransactionModel TaxResponse { get; set; }
    }
}
