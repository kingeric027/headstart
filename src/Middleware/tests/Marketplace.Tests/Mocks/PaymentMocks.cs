using Marketplace.Common.Models.Marketplace;
using ordercloud.integrations.cardconnect;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Tests.Mocks
{
    public static class PaymentMocks
    {
        public static List<MarketplacePayment> Payments(params MarketplacePayment[] payments)
        {
            return new List<MarketplacePayment>(payments);
        }

        public static ListPage<MarketplacePayment> EmptyPaymentsList()
        {
            var items = new List<MarketplacePayment>();
            return new ListPage<MarketplacePayment>
            {
                Items = items
            };
        }

        public static ListPage<MarketplacePayment> PaymentList(params MarketplacePayment[] payments)
        {
            return new ListPage<MarketplacePayment>
            {
                Items = new List<MarketplacePayment>(payments)
            };
        }

        public static MarketplacePayment CCPayment(string creditCardID, decimal? amount = null, string id = "mockCCPaymentID", bool accepted = true)
        {
            return new MarketplacePayment
            {
                ID = id,
                Type = PaymentType.CreditCard,
                CreditCardID = creditCardID,
                Amount = amount,
                Accepted = accepted
            };
        }

        public static MarketplacePayment POPayment(decimal? amount = null, string id = "mockPoPaymentID")
        {
            return new MarketplacePayment
            {
                ID = id,
                Type = PaymentType.PurchaseOrder,
                Amount = amount
            };
        }
    }
}
