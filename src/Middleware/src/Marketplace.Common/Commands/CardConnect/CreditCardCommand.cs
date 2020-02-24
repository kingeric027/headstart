using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Helpers;
using Marketplace.Common.Mappers.CardConnect;
using Marketplace.Common.Services.CardConnect;
using Marketplace.Helpers;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Models;
using Marketplace.Models.Misc;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
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
		private readonly IOrderCloudClient _privilegedOC;

		public CreditCardCommand(AppSettings settings, ICardConnectService card, IOrderCloudClient oc)
        {
            _card = card;
            _settings = settings;
            _oc = new OrderCloudClient(new OrderCloudClientConfig()
            {
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                AuthUrl = settings.OrderCloudSettings.AuthUrl
            });
			_privilegedOC = oc;
		}

        public async Task<CreditCard> TokenizeAndSave(string buyerID, CreditCardToken card, VerifiedUserContext user)
        {
            return await _oc.CreditCards.CreateAsync(buyerID, await Tokenize(card), user.AccessToken);
        }

        public async Task<BuyerCreditCard> MeTokenizeAndSave(CreditCardToken card, VerifiedUserContext user)
        {
			return await _oc.Me.CreateCreditCardAsync(await MeTokenize(card), user.AccessToken);
        }

		public async Task<Payment> AuthorizePayment(CreditCardPayment payment, VerifiedUserContext user)
        {
			Require.That(payment.CreditCardID != null || payment.CreditCardDetails != null, 
				new ErrorCode("Missing credit card info", 400, "Must include CreditCardDetails or CreditCardID."));

			var cc = await GetMeCardDetails(payment, user);

            Require.That(cc.Token != null, new ErrorCode("Invalid credit card token", 400, "Credit card must have valid authorization token"));

			var order = await _privilegedOC.Orders.GetAsync(OrderDirection.Incoming, payment.OrderID);

			var paymentlist = await _privilegedOC.Payments.ListAsync<Payment>(OrderDirection.Incoming, payment.OrderID, builder => builder.AddFilter(p => p.CreditCardID == payment.CreditCardID));
			if (paymentlist.Meta.TotalCount == 0)
				throw new ApiErrorException(new ErrorCode("Required", 404, $"Unable to find Payment on Order {payment.OrderID} with CreditCardID {payment.CreditCardID}"), payment.OrderID);
			var ocPayment = paymentlist.Items.First();

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
			ocPayment = await _privilegedOC.Payments.PatchAsync(OrderDirection.Incoming, order.ID, ocPayment.ID, new PartialPayment { Accepted = true });
            var trans = await _privilegedOC.Payments.CreateTransactionAsync(OrderDirection.Incoming, order.ID, ocPayment.ID,
                CardConnectMapper.Map(order, ocPayment, call));
            return trans;
        }

		private async Task<BuyerCreditCard> GetMeCardDetails(CreditCardPayment payment, VerifiedUserContext user)
		{
			if (payment.CreditCardID != null)
			{
				return await _oc.Me.GetCreditCardAsync<BuyerCreditCard>(payment.CreditCardID, user.AccessToken);
			}
			return await MeTokenize(payment.CreditCardDetails);
		}

		private async Task<BuyerCreditCard> MeTokenize(CreditCardToken card)
		{
			var auth = await _card.Tokenize(CardConnectMapper.Map(card));
			return BuyerCreditCardMapper.Map(card, auth);
		}

		private async Task<CreditCard> Tokenize(CreditCardToken card)
		{
			var auth = await _card.Tokenize(CardConnectMapper.Map(card));
			return CreditCardMapper.Map(card, auth);
		}
	}
}
