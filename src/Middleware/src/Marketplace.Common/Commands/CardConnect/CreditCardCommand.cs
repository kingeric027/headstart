using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Helpers;
using Marketplace.Common.Mappers.CardConnect;
using Marketplace.Common.Models.CardConnect;
using Marketplace.Common.Services.CardConnect;
using Marketplace.Helpers;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.CardConnect
{
    public interface ICreditCardCommand
    {
        //Task<CreditCardAuthorization> Authorize(CreditCardAuthorization auth);
        Task<BuyerCreditCard> MeTokenizeAndSave(CreditCardToken card, VerifiedUserContext user);
        Task<CreditCard> TokenizeAndSave(string buyerID, CreditCardToken card, VerifiedUserContext user);
        Task<Payment> AuthorizePayment(CreditCardPayment payment, VerifiedUserContext user);
    }

    public class CreditCardCommand : ICreditCardCommand
    {
        private readonly ICardConnectService _card;
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;

		public CreditCardCommand(AppSettings settings, ICardConnectService card)
        {
            _card = card;
            _settings = settings;
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = "https://api.ordercloud.io",
                AuthUrl = "https://auth.ordercloud.io"
            });
		}

        public async Task<CreditCard> TokenizeAndSave(string buyerID, CreditCardToken card, VerifiedUserContext user)
        {
            var auth = await _card.Tokenize(CardConnectMapper.Map(card));
            var cc = await _oc.CreditCards.CreateAsync(buyerID, CreditCardMapper.Map(card, auth), user.AccessToken);
            return cc;
        }

        public async Task<BuyerCreditCard> MeTokenizeAndSave(CreditCardToken card, VerifiedUserContext user)
        {
            var auth = await _card.Tokenize(CardConnectMapper.Map(card));
            var cc = await _oc.Me.CreateCreditCardAsync(BuyerCreditCardMapper.Map(card, auth), user.AccessToken);
            return cc;
        }


        public async Task<Payment> AuthorizePayment(CreditCardPayment payment, VerifiedUserContext user)
        {
            var cc = await _oc.Me.GetCreditCardAsync<BuyerCreditCard>(payment.CreditCardID, user.AccessToken);
            Require.That(cc.Token != null, new ErrorCode("Invalid credit card token", 400, "Credit card must have valid authorization token"));

			var orderlist = await _oc.Me.ListOrdersAsync<Order>(builder => builder.AddFilter(o => o.ID == payment.OrderID), accessToken: user.AccessToken);
			if (orderlist.Meta.TotalCount == 0)
				throw new ApiErrorException(new ErrorCode("Required", 404, "Unable to find Order"), payment.OrderID);
			var order = orderlist.Items.First();

			Require.That(!order.IsSubmitted, new ErrorCode("Invalid Order Status", 400, "Order has already been submitted"));
            Require.That(order.BillingAddress != null || order.BillingAddressID != null, new ErrorCode("Invalid Bill Address", 400, "Order must supply valid billing address for credit card verification"));

            if (order.BillingAddress == null)
            {
                var address = await _oc.Me.GetAddressAsync(order.BillingAddressID, user.AccessToken);
                order.BillingAddress = new Address()
                {
                    AddressName = address.AddressName,
                    City = address.City,
                    Country = address.Country,
                    ID = address.ID,
                    DateCreated = address.DateCreated,
                    Phone = address.Phone,
                    FirstName = address.FirstName,
                    LastName = address.LastName,
                    Street1 = address.Street1,
                    CompanyName = address.CompanyName,
                    Street2 = address.Street2,
                    State = address.State,
                    Zip = address.Zip,
                    xp = address.xp
                };
            }

            var call = await _card.AuthWithoutCapture(CardConnectMapper.Map(cc, order, payment));
            var p = await _oc.Payments.CreateAsync(OrderDirection.Outgoing, order.ID, CardConnectMapper.Map(call, payment), user.AccessToken);
            var trans = await _oc.Payments.CreateTransactionAsync(OrderDirection.Outgoing, order.ID, p.ID,
                CardConnectMapper.Map(order, p, call), user.AccessToken);
            return trans;
        }
    }
}
