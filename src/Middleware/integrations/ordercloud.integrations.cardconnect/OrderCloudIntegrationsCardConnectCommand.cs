using System.Linq;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace ordercloud.integrations.cardconnect
{
	public interface IOrderCloudIntegrationsCardConnectCommand
	{
		//Task<CreditCardAuthorization> Authorize(CreditCardAuthorization auth);
		Task<BuyerCreditCard> MeTokenizeAndSave(OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user);
		Task<CreditCard> TokenizeAndSave(string buyerID, OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user);
		Task<Payment> AuthorizePayment(OrderCloudIntegrationsCreditCardPayment payment, VerifiedUserContext user, string merchantID);
    }

	public class OrderCloudIntegrationsCardConnectCommand : IOrderCloudIntegrationsCardConnectCommand
	{
		private readonly IOrderCloudIntegrationsCardConnectService _cardConnect;
		private readonly IOrderCloudClient _oc; 

		public OrderCloudIntegrationsCardConnectCommand(IOrderCloudIntegrationsCardConnectService card, IOrderCloudClient oc)
		{
			_cardConnect = card;
			_oc = oc;
		}

		public async Task<CreditCard> TokenizeAndSave(string buyerID, OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user)
		{
			var creditCard = await _oc.CreditCards.CreateAsync(buyerID, await Tokenize(card), user.AccessToken);
			return creditCard;
		}

		public async Task<BuyerCreditCard> MeTokenizeAndSave(OrderCloudIntegrationsCreditCardToken card, VerifiedUserContext user)
		{
			var buyerCreditCard = await _oc.Me.CreateCreditCardAsync(await MeTokenize(card), user.AccessToken);
			return buyerCreditCard;
		}

		public async Task<Payment> AuthorizePayment(
			OrderCloudIntegrationsCreditCardPayment payment,
            VerifiedUserContext user,
			string merchantID
		)
		{
			Require.That((payment.CreditCardID != null) || (payment.CreditCardDetails != null),
				new ErrorCode("Missing credit card info", 400, "Request must include either CreditCardDetails or CreditCardID"));

			var cc = await GetMeCardDetails(payment, user);

			Require.That(payment.IsValidCvv(cc), new ErrorCode("Invalid CVV", 400, "CVV is required for Credit Card Payment"));
			Require.That(cc.Token != null, new ErrorCode("Invalid credit card token", 400, "Credit card must have valid authorization token"));
			Require.That(cc.xp.CCBillingAddress != null, new ErrorCode("Invalid Bill Address", 400, "Credit card must have a billing address"));

			var orderWorksheet = await _oc.IntegrationEvents.GetWorksheetAsync(OrderDirection.Incoming, payment.OrderID);
			var order = orderWorksheet.Order;

			Require.That(!order.IsSubmitted, new ErrorCode("Invalid Order Status", 400, "Order has already been submitted"));

			var ccAmount = GetAmountToCharge(orderWorksheet);

			var ocPayment = await _oc.Payments.GetAsync<Payment>(OrderDirection.Incoming, payment.OrderID, payment.PaymentID);
			var call = await _cardConnect.AuthWithoutCapture(CardConnectMapper.Map(cc, order, payment, merchantID, ccAmount));
			ocPayment = await _oc.Payments.PatchAsync(OrderDirection.Incoming, order.ID, ocPayment.ID, new PartialPayment { Accepted = true, Amount = ccAmount });
			var transaction = await _oc.Payments.CreateTransactionAsync(OrderDirection.Incoming, order.ID, ocPayment.ID,
				CardConnectMapper.Map(order, ocPayment, call));
			return transaction;
		}

		private decimal GetAmountToCharge(OrderWorksheet orderWorksheet)
        {
			var purchaseOrderLineItems = orderWorksheet.LineItems.Where(li => li.Product.xp.ProductType == "PurchaseOrder");
			var purchaseOrderSubtotal =
				purchaseOrderLineItems
				.Select(li => li.Quantity * li.UnitPrice)
				.Sum();

			var puchaseOrderLineItemIDs = purchaseOrderLineItems.Select(li => li.ID);
			var purchaseOrderShipEstimates = orderWorksheet.ShipEstimateResponse.ShipEstimates.Where(shipEstimate =>
			{
				return shipEstimate.ShipEstimateItems.Any(item => puchaseOrderLineItemIDs.Contains(item.LineItemID));
			});

			var poShippingCosts = purchaseOrderShipEstimates.Select(shipEstimate =>
			{
				var selectedShipMethod = shipEstimate.ShipMethods.First(ShipMethod => ShipMethod.ID == shipEstimate.SelectedShipMethodID);
				return selectedShipMethod.Cost;
			});

			var poShippingCost = poShippingCosts.Sum();

			var purchaseOrderTotal = (purchaseOrderSubtotal ?? 0) + poShippingCost;

			return orderWorksheet.Order.Total - purchaseOrderTotal;
		}
		
		private async Task<BuyerCreditCard> GetMeCardDetails(OrderCloudIntegrationsCreditCardPayment payment, VerifiedUserContext user)
		{
			if (payment.CreditCardID != null)
			{
				return await _oc.Me.GetCreditCardAsync<BuyerCreditCard>(payment.CreditCardID, user.AccessToken);
			}
			return await MeTokenize(payment.CreditCardDetails);
		}

		private async Task<BuyerCreditCard> MeTokenize(OrderCloudIntegrationsCreditCardToken card)
		{
			var auth = await _cardConnect.Tokenize(CardConnectMapper.Map(card));
			return BuyerCreditCardMapper.Map(card, auth);
		}

		private async Task<CreditCard> Tokenize(OrderCloudIntegrationsCreditCardToken card)
		{
			var auth = await _cardConnect.Tokenize(CardConnectMapper.Map(card));
			return CreditCardMapper.Map(card, auth);
		}
	}
}
