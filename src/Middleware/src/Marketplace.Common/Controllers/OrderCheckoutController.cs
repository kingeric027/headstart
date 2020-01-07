using Avalara.AvaTax.RestClient;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.FreightPop;
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
		private readonly IMockShippingCacheService _shippingCache;
		private readonly IAvataxService _taxService;

		// Needs more authentication. These methods should only work for a specific user's orders.

		public OrderCheckoutController(AppSettings settings, IOrderCheckoutCommand command, IMockShippingCacheService shippingCache, IAvataxService taxService) : base() {
			_checkoutCommand = command;
			_shippingCache = shippingCache;
			_taxService = taxService;
		}

		[HttpPost, Route("{orderID}/shipping-quote"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<IEnumerable<ShippingOptions>> GenerateShippingQuotesAsync(string orderID)
		{
			return await _checkoutCommand.GenerateShippingQuotesAsync(orderID);
		}

		[HttpGet, Route("{orderID}/shipping-quote/{quoteID}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<ShippingRate> GetSavedShippingQuoteAsync(string orderID, string quoteID)
		{
			return await _shippingCache.GetSavedShippingQuoteAsync(orderID, quoteID);
		}

		[HttpPut, Route("{orderID}/shipping-selection"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, [FromBody] ShippingSelection shippingSelection)
		{
			return await _checkoutCommand.SetShippingSelectionAsync(orderID, shippingSelection);
		}

		[HttpPost, Route("{orderID}/tax-transaction"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> CalcTaxAndPatchOrderAsync(string orderID)
		{
			return await _checkoutCommand.CalcTaxAndPatchOrderAsync(orderID);
		}

		[HttpGet, Route("{orderID}/tax-transaction/{transactionID}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<TransactionModel> GetSavedTaxTransactionAsync(string orderID, string transactionID)
		{
			return await _taxService.GetTaxTransactionAsync(transactionID);
		}
	}
}
