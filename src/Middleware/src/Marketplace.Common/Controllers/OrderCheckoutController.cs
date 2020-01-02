using Avalara.AvaTax.RestClient;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
	[Route("orders")]
	public class OrderCheckoutController: Controller
	{
		private readonly IOrderCheckoutCommand _checkoutCommand;
		private readonly IMockShippingService _shippingService;
		private readonly IAvataxService _taxService;

		// Needs more authentication. These methods should only work for a specific user's orders.

		public OrderCheckoutController(AppSettings settings, IOrderCheckoutCommand command, IMockShippingService shipping, IAvataxService taxService) : base() {
			_checkoutCommand = command;
			_shippingService = shipping;
			_taxService = taxService;
		}

		[HttpGet, Route("{orderID}/shipping-quote"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<IEnumerable<ShippingOptions>> GenerateShippingQuotes(string orderID)
		{
			return await _checkoutCommand.GenerateShippingQuotes(orderID);
		}

		[HttpGet, Route("{orderID}/shipping-quote/{quoteID}")]
		public async Task<MockShippingQuote> GetSavedShippingQuote(string orderID, string quoteID)
		{
			return await _shippingService.GetSavedShipmentQuote(orderID, quoteID);
		}

		[HttpGet, Route("{orderID}/tax-transaction/{transactionID}")]
		public async Task<TransactionModel> GetSavedTaxTransaction(string orderID, string transactionID)
		{
			return await _taxService.GetTaxTransactionAsync(transactionID);
		}

		[HttpPost, Route("{orderID}/calculate-tax"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> CalculateTax(string orderID)
		{
			return await _checkoutCommand.CalculateTax(orderID);
		}

		[HttpPost, Route("{orderID}/set-shipping-quote"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> SetShippingQuote(string orderID, [FromBody] ShippingSelection shippingSelection)
		{
			return await _checkoutCommand.SetShippingSelection(orderID, shippingSelection);
		}
	}
}
