using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Mappers.CardConnect;
using Marketplace.Common.Services.CardConnect;
using Marketplace.Helpers;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands
{
    public interface ICreditCardCommand
    {
        //Task<CreditCardAuthorization> Authorize(CreditCardAuthorization auth);
        Task<BuyerCreditCard> MeTokenizeAndSave(CreditCardToken card, VerifiedUserContext user);
        Task<CreditCard> TokenizeAndSave(string buyerID, CreditCardToken card, VerifiedUserContext user);
        Task<Payment> AuthorizePayment(string paymentID, CreditCardPayment payment, VerifiedUserContext user);
    }

    public class CreditCardCommand : ICreditCardCommand
    {
        private readonly ICardConnectService _cardConnect;
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;

		public CreditCardCommand(AppSettings settings, ICardConnectService card, IOrderCloudClient oc)
        {
			_cardConnect = card;
            _settings = settings;
			_oc = oc;
        }

        public async Task<CreditCard> TokenizeAndSave(string buyerID, CreditCardToken card, VerifiedUserContext user)
        {
			var creditCard = await _oc.CreditCards.CreateAsync(buyerID, await Tokenize(card), user.AccessToken);
			return creditCard;
		}

        public async Task<BuyerCreditCard> MeTokenizeAndSave(CreditCardToken card, VerifiedUserContext user)
        {
			var buyerCreditCard = await _oc.Me.CreateCreditCardAsync(await MeTokenize(card), user.AccessToken);
			return buyerCreditCard;
		}

		public async Task<Payment> AuthorizePayment(string paymentID, CreditCardPayment payment, VerifiedUserContext user)
        {
			Require.That((payment.CreditCardID != null) ^ (payment.CreditCardDetails != null), 
				new ErrorCode("Missing credit card info", 400, "Request must include either CreditCardDetails or CreditCardID, but not both."));

			var cc = await GetMeCardDetails(payment, user);
            
            Require.That(payment.IsValidCvv(cc), new ErrorCode("Invalid CVV", 400, "CVV is required for Credit Card Payment"));
            Require.That(cc.Token != null, new ErrorCode("Invalid credit card token", 400, "Credit card must have valid authorization token"));
			Require.That(cc.xp.CCBillingAddress != null, new ErrorCode("Invalid Bill Address", 400, "Credit card must have a billing address"));

			var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, payment.OrderID);

			Require.That(!order.IsSubmitted, new ErrorCode("Invalid Order Status", 400, "Order has already been submitted"));

			var ocPayment = await _oc.Payments.GetAsync<Payment>(OrderDirection.Incoming, payment.OrderID, paymentID);

            var call = await _cardConnect.AuthWithoutCapture(CardConnectMapper.Map(cc, order, payment));
			ocPayment = await _oc.Payments.PatchAsync(OrderDirection.Incoming, order.ID, ocPayment.ID, new PartialPayment { Accepted = true });
            var transaction = await _oc.Payments.CreateTransactionAsync(OrderDirection.Incoming, order.ID, ocPayment.ID,
                CardConnectMapper.Map(order, ocPayment, call));
            return transaction;
        }

		private async Task<MarketplaceBuyerCreditCard> GetMeCardDetails(CreditCardPayment payment, VerifiedUserContext user)
		{
			if (payment.CreditCardID != null)
			{
				return await _oc.Me.GetCreditCardAsync<MarketplaceBuyerCreditCard>(payment.CreditCardID, user.AccessToken);
			}
			return await MeTokenize(payment.CreditCardDetails);
		}

		private async Task<MarketplaceBuyerCreditCard> MeTokenize(CreditCardToken card)
		{
			var auth = await _cardConnect.Tokenize(CardConnectMapper.Map(card));
			return BuyerCreditCardMapper.Map(card, auth);
		}

		private async Task<MarketplaceCreditCard> Tokenize(CreditCardToken card)
		{
			var auth = await _cardConnect.Tokenize(CardConnectMapper.Map(card));
			return CreditCardMapper.Map(card, auth);
		}
	}
}
