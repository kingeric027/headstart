using System;
using System.Collections.Generic;
using System.Text;
using Avalara.AvaTax.RestClient;
using OrderCloud.SDK;

namespace Marketplace.Common.Models.Marketplace
{
    public class MarketplaceOrderCalculateResponse : OrderCalculateResponse<OrderCalculateResponseXp>
    {

    }

    public class OrderCalculateResponseXp
    {
        public TransactionModel TaxResponse { get; set; }
    }
}
