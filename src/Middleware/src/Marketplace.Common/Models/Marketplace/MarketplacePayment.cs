using ordercloud.integrations.cardconnect;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models.Marketplace
{
    [SwaggerModel]
    public class MarketplacePayment : Payment<PaymentXP, MarketplacePaymentTransaction>
    {

    }

    [SwaggerModel]
    public class MarketplacePaymentTransaction: PaymentTransaction<TransactionXP>
    {

    }

    [SwaggerModel]
    public class PaymentXP
    {
        public string partialAccountNumber { get; set; }
        public string cardType { get; set; }
    }

    [SwaggerModel]
    public class TransactionXP
    {
        public CardConnectAuthorizationResponse CardConnectResponse { get; set; }
    }
}
