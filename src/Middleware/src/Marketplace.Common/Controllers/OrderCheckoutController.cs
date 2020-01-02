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
	public class OrderCheckoutController : BaseController
	{
		private readonly IOrderCheckoutCommand _checkoutCommand;
		private readonly IMockShippingService _shippingService;
		private readonly IAvataxService _taxService;

		public OrderCheckoutController(AppSettings settings, IOrderCheckoutCommand command, IMockShippingService shipping, IAvataxService taxService) : base(settings) {
			_checkoutCommand = command;
			_shippingService = shipping;
			_taxService = taxService;
		}

		[HttpGet, Route("{orderID}/shipping-quote")]
		public async Task<IEnumerable<ShippingOptions>> GenerateShippingQuotes(string orderID)
		{
			return await _checkoutCommand.GenerateShippingQuotes(orderID);
		}

		[HttpGet, Route("{orderID}/shipping-quote/{quoteID}")]
		public async Task<MockShippingQuote> GetSavedShippingQuote(string orderID, string quoteID)
		{
			return await _shippingService.GetSavedShipmentQuote(orderID, quoteID);
		}

		// How to do authentication?
		[HttpGet, Route("tax-transaction/{transactionID}")]
		public async Task<TransactionModel> GetSavedTaxTransaction(string transactionID)
		{
			return await _taxService.GetTaxTransactionAsync(transactionID);
		}

		[HttpPut, Route("{orderID}/shipping-quote")]
		public async Task<MarketplaceOrder> SetShippingAndTax(string orderID, [FromBody] ShippingSelection shippingSelection)
		{
			return await _checkoutCommand.SetShippingSelection(orderID, shippingSelection);
		}


	}
}
