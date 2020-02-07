using System;
using System.Globalization;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models.CardConnect;
using Marketplace.Common.Services.CardConnect.Models;
using Marketplace.Helpers.Extensions;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers.CardConnect
{
    public static class CreditCardMapper
    {
        public static CreditCard Map(CreditCardToken card, AccountResponse response)
        {
            var cc = new CreditCard()
            {
                CardType = card.CardType,
                CardholderName = card.CardholderName,
                ExpirationDate = card.ExpirationDate.ToDateTime(),
                PartialAccountNumber = card.AccountNumber.ToCreditCardDisplay(),
                Token = response.token
            };
            return cc;
        }
    }

    public static class BuyerCreditCardMapper
    {
        public static BuyerCreditCard Map(CreditCardToken card, AccountResponse response)
        {
            var cc = new BuyerCreditCard()
            {
                CardType = card.CardType,
                CardholderName = card.CardholderName,
                ExpirationDate = card.ExpirationDate.ToDateTime(),
                PartialAccountNumber = card.AccountNumber.ToCreditCardDisplay(),
                Token = response.token
            };
            return cc;
        }
    }

    public static class CardConnectMapper
    {
        public static AccountRequest Map(CreditCardToken card)
        {
            var acct = new AccountRequest()
            {
                account = card.AccountNumber
            };
            return acct;
        }

        //public static CreditCardToken Map(AccountResponse resp, CreditCardToken request)
        //{
        //    var cc = new CreditCardToken()
        //    {
        //        Token = resp.token,
        //        PartialAccountNumber = request.AccountNumber.ToCreditCardDisplay(),
        //        AccountNumber = request.AccountNumber.ToCreditCardDisplay(),
        //        CardholderName = request.CardholderName
        //    };
        //    return cc;
        //}

        public static AuthorizationRequest Map(BuyerCreditCard card, Order order, CreditCardPayment payment)
        {
            var req = new AuthorizationRequest()
            {
                name = $"{order.BillingAddress.FirstName} {order.BillingAddress.LastName}",
                account = card.Token,
                address = order.BillingAddress.Street1,
                amount = order.Total.ToString(CultureInfo.InvariantCulture),
                //capture = auth.capture,
                //bin = auth.bin,
                city = order.BillingAddress.City,
                country = order.BillingAddress.Country,
                currency = payment.Currency,
                cvv2 = payment.CVV,
                expiry = $"{card.ExpirationDate.Value:MMyyyy}",
                merchid = payment.MerchantID,
                orderid = order.ID,
                postal = order.BillingAddress.Zip,
                region = order.BillingAddress.State
            };
            return req;
        }

        public static PaymentTransaction Map(Order order, Payment payment, AuthorizationResponse response)
        {
            var t = new PaymentTransaction()
            {
                Amount = payment.Amount,
                DateExecuted = DateTime.Now,
                ResultCode = response.authcode,
                ResultMessage = payment.Description,
                Succeeded = payment.Accepted == true,
                Type = payment.Type.ToString()
            };
            return t;
        }

        //public static CreditCardAuthorization Map(AuthorizationResponse response, CreditCardAuthorization request)
        //{
        //    var cc = new CreditCardAuthorization()
        //    {
        //        Token = response.token,
        //        Status = response.respstat.ToResponseStatus(),
        //        Amount = response.amount,
        //        Account = response.account,
        //        CVV = request.CVV,
        //        ReferenceNumber = response.retref,
        //        ExpirationDate = response.expiry,
        //        MerchantID = response.merchid,
        //        ResponseCode = response.respcode,
        //        ResponseText = response.resptext,
        //        ResponseProcessor = response.respproc,
        //        AVSResponseCode = response.avsresp,
        //        CVVResponseCode = response.cvvresp.ToCvvResponse(),
        //        BinType = response.bintype.ToBinType(),
        //        AuthorizationCode = response.authcode,
        //        Receipt = response.receipt,
        //        CommercialCard = response.commcard == "Y",
        //        OrderID = request.OrderID,
        //        Currency = request.Currency,
        //        CardHolderName = request.CardHolderName,
        //        CardHolderEmail = request.CardHolderEmail,
        //        Address = request.Address,
        //        City = request.City,
        //        Region = request.Region,
        //        Country = request.Country,
        //        PostalCode = request.PostalCode
        //    };
        //    return cc;
        //}
    }

    public static class CreditCardAuthorizationExtensions
    {
        public static DateTime ToDateTime(this string value)
        {
            var month = value.Substring(0, 2).To<int>();
            if (value.Length == 4)
            {
                var year = $"20{value.Substring(2, 2)}".To<int>();
                return new DateTime(year, month, 1);
            }
            else if (value.Length == 6)
            {
                var year = value.Substring(2, 4).To<int>();
                return new DateTime(year, month, 1);
            }

            throw new Exception("Invalid format: MMYY MMYYYY");
        }

        public static ResponseStatus ToResponseStatus(this string value)
        {
            switch (value)
            {
                case "A":
                    return ResponseStatus.Approved;
                case "B":
                    return ResponseStatus.Retry;
                case "C":
                    return ResponseStatus.Declined;
                default:
                    throw new Exception($"Invalid response code: {value}");
            }
        }

        public static CVVResponse ToCvvResponse(this string value)
        {
            switch (value)
            {
                case "M":
                case "X": // not documented but returned on successful calls in testing
                    return CVVResponse.Valid;
                case "N":
                    return CVVResponse.Invalid;
                case "P":
                    return CVVResponse.NotProcessed;
                case "S":
                    return CVVResponse.NotPresent;
                case "U":
                    return CVVResponse.NotCertified;
                default:
                    throw new Exception($"Invalid response code: {value}");
            }
        }

        public static BinType ToBinType(this string value)
        {
            switch (value)
            {
                case "Corp": return BinType.Corporate;
                case "FSA+Prepaid": return BinType.FSAPrepaid;
                case "GSA+Purchase": return BinType.GSAPurchase;
                case "Prepaid": return BinType.Prepaid;
                case "Prepaid+Corp": return BinType.PrepaidCorporate;
                case "Prepaid+Purchase": return BinType.PrepaidPurchase;
                case "Purchase": return BinType.Purchase;
                default: return BinType.Invalid;
            }
        }
    }
}
