﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Headstart.Common;
using Headstart.Common.Models.Marketplace;
using Headstart.Common.Services;
using Headstart.Common.Services.ShippingIntegration.Models;
using Headstart.Models;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using Polly;
using Polly.Retry;

namespace ordercloud.integrations.cardconnect
{
	public interface ICreditCardCommand
	{
		Task<BuyerCreditCard> MeTokenizeAndSave(OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user);
		Task<CreditCard> TokenizeAndSave(string buyerID, OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user);
		Task<Payment> AuthorizePayment(OrderCloudIntegrationsCreditCardPayment payment, string userToken, string merchantID);
		Task VoidPaymentAsync(HSPayment payment, HSOrder order, string userToken);

	}

	public class CreditCardCommand : ICreditCardCommand
	{
		private readonly IOrderCloudIntegrationsCardConnectService _cardConnect;
		private readonly IOrderCloudClient _oc;
		private readonly IOrderCalcService _orderCalc;
		private readonly ISebExchangeRatesService _sebExchangeRates;
		private readonly ISupportAlertService _supportAlerts;
		private readonly AppSettings _settings;

		public CreditCardCommand(
			IOrderCloudIntegrationsCardConnectService card,
			IOrderCloudClient oc,
			IOrderCalcService orderCalc,
			ISebExchangeRatesService sebExchangeRates,
			ISupportAlertService supportAlerts,
			AppSettings settings
		)
		{
			_cardConnect = card;
			_oc = oc;
			_orderCalc = orderCalc;
			_sebExchangeRates = sebExchangeRates;
			_supportAlerts = supportAlerts;
			_settings = settings;
		}

		public async Task<CreditCard> TokenizeAndSave(string buyerID, OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user)
		{
			var creditCard = await _oc.CreditCards.CreateAsync(buyerID, await Tokenize(card, user.AccessToken), user.AccessToken);
			return creditCard;
		}

		public async Task<BuyerCreditCard> MeTokenizeAndSave(OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user)
		{
			var buyerCreditCard = await _oc.Me.CreateCreditCardAsync(await MeTokenize(card, user.AccessToken), user.AccessToken);
			return buyerCreditCard;
		}

		public async Task<Payment> AuthorizePayment(
			OrderCloudIntegrationsCreditCardPayment payment,
            string userToken,
			string merchantID
		)
		{
			Require.That((payment.CreditCardID != null) || (payment.CreditCardDetails != null),
				new ErrorCode("CreditCard.CreditCardAuth", 400, "Request must include either CreditCardDetails or CreditCardID"));

			var cc = await GetMeCardDetails(payment, userToken);

			Require.That(payment.IsValidCvv(cc), new ErrorCode("CreditCardAuth.InvalidCvv", 400, "CVV is required for Credit Card Payment"));
			Require.That(cc.Token != null, new ErrorCode("CreditCardAuth.InvalidToken", 400, "Credit card must have valid authorization token"));
			Require.That(cc.xp.CCBillingAddress != null, new ErrorCode("Invalid Bill Address", 400, "Credit card must have a billing address"));

			var orderWorksheet = await _oc.IntegrationEvents.GetWorksheetAsync<HSOrderWorksheet>(OrderDirection.Incoming, payment.OrderID);
			var order = orderWorksheet.Order;

			Require.That(!order.IsSubmitted, new ErrorCode("CreditCardAuth.AlreadySubmitted", 400, "Order has already been submitted"));

			var ccAmount = _orderCalc.GetCreditCardTotal(orderWorksheet);

			var ocPaymentsList = (await _oc.Payments.ListAsync<HSPayment>(OrderDirection.Incoming, payment.OrderID, filters: "Type=CreditCard" ));
			var ocPayments = ocPaymentsList.Items;
			var ocPayment = ocPayments.Any() ? ocPayments[0] : null;
			if(ocPayment == null)
            {
				throw new OrderCloudIntegrationException(new ApiError
				{
					ErrorCode = "Payment.MissingCreditCardPayment",
					Message = "Order is missing credit card payment"
				});
            }
            try
            {
				if(ocPayment?.Accepted == true)
                {
					if(ocPayment.Amount == ccAmount)
                    {
						return ocPayment;
                    } else
                    {
						await VoidPaymentAsync(ocPayment, order, userToken);
                    }
                }
                var call = await _cardConnect.AuthWithoutCapture(CardConnectMapper.Map(cc, order, payment, merchantID, ccAmount));
                ocPayment = await WithRetry().ExecuteAsync(() => _oc.Payments.PatchAsync<HSPayment>(OrderDirection.Incoming, order.ID, ocPayment.ID, new PartialPayment { Accepted = true, Amount = ccAmount }));
                return await WithRetry().ExecuteAsync(() => _oc.Payments.CreateTransactionAsync(OrderDirection.Incoming, order.ID, ocPayment.ID, CardConnectMapper.Map(ocPayment, call)));
            }
            catch (CreditCardAuthorizationException ex)
            {
                ocPayment = await WithRetry().ExecuteAsync(() => _oc.Payments.PatchAsync<HSPayment>(OrderDirection.Incoming, order.ID, ocPayment.ID, new PartialPayment { Accepted = false, Amount = ccAmount }));
				await WithRetry().ExecuteAsync(() => _oc.Payments.CreateTransactionAsync(OrderDirection.Incoming, order.ID, ocPayment.ID, CardConnectMapper.Map(ocPayment, ex.Response)));
                throw new OrderCloudIntegrationException(new ApiError()
                {
                    Data = ex.Response,
                    Message = ex.ApiError.Message,
                    ErrorCode = $"CreditCardAuth.{ex.ApiError.ErrorCode}"
                });
			}
		}

		public async Task VoidPaymentAsync(HSPayment payment, HSOrder order, string userToken)
        {
			var transactionID = "";
			try
			{
				if (payment.Accepted == true)
				{
					var transaction = payment.Transactions
										.Where(x => x.Type == "CreditCard")
										.OrderBy(x => x.DateExecuted)
										.LastOrDefault(t => t.Succeeded);
					var retref = transaction?.xp?.CardConnectResponse?.retref;
					if (retref != null)
					{
						transactionID = transaction.ID;
						var userCurrency = await _sebExchangeRates.GetCurrencyForUser(userToken);
						var response = await _cardConnect.VoidAuthorization(new CardConnectVoidRequest
						{
							currency = userCurrency.ToString(),
							merchid = GetMerchantIDAsync(userCurrency),
							retref = transaction.xp.CardConnectResponse.retref
						});
						await WithRetry().ExecuteAsync(() => _oc.Payments.CreateTransactionAsync(OrderDirection.Incoming, order.ID, payment.ID, CardConnectMapper.Map(payment, response)));
					}
				}
			}
			catch (CreditCardVoidException ex)
			{

				await _supportAlerts.VoidAuthorizationFailed(payment, transactionID, order, ex);
				await WithRetry().ExecuteAsync(() => _oc.Payments.CreateTransactionAsync(OrderDirection.Incoming, order.ID, payment.ID, CardConnectMapper.Map(payment, ex.Response)));
				throw new OrderCloudIntegrationException(new ApiError
				{
					ErrorCode = "Payment.FailedToVoidAuthorization",
					Message = ex.ApiError.Message
				});
			}
		}

		private string GetMerchantIDAsync(CurrencySymbol userCurrency)
		{
			if (userCurrency == CurrencySymbol.USD)
				return _settings.CardConnectSettings.UsdMerchantID;
			else if (userCurrency == CurrencySymbol.CAD)
				return _settings.CardConnectSettings.CadMerchantID;
			else
				return _settings.CardConnectSettings.EurMerchantID;
		}

		private async Task<CardConnectBuyerCreditCard> GetMeCardDetails(OrderCloudIntegrationsCreditCardPayment payment, string userToken)
		{
			if (payment.CreditCardID != null)
			{
				return await _oc.Me.GetCreditCardAsync<CardConnectBuyerCreditCard>(payment.CreditCardID, userToken);
			}
			return await MeTokenize(payment.CreditCardDetails, userToken);
		}

		private async Task<CardConnectBuyerCreditCard> MeTokenize(OrderCloudIntegrationsCreditCardToken card, string userToken)
		{
			var userCurrency = await _sebExchangeRates.GetCurrencyForUser(userToken);
			var auth = await _cardConnect.Tokenize(CardConnectMapper.Map(card, userCurrency.ToString()));
			return BuyerCreditCardMapper.Map(card, auth);
		}

		private async Task<CreditCard> Tokenize(OrderCloudIntegrationsCreditCardToken card, string userToken)
		{
			var userCurrency = await _sebExchangeRates.GetCurrencyForUser(userToken);
			var auth = await _cardConnect.Tokenize(CardConnectMapper.Map(card, userCurrency.ToString()));
			return CreditCardMapper.Map(card, auth);
		}

		private AsyncRetryPolicy WithRetry()
		{
			// retries three times on 500 errors or timeout
			// waits two seconds in-between failures
			return Policy
				.Handle<OrderCloudException>(e => e.HttpStatus == HttpStatusCode.InternalServerError || e.HttpStatus == HttpStatusCode.RequestTimeout)
				.WaitAndRetryAsync(new[] {
					TimeSpan.FromSeconds(2),
					TimeSpan.FromSeconds(2),
					TimeSpan.FromSeconds(2),
				});
		}
	}
}
