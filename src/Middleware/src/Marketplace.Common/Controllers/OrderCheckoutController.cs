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

		public OrderCheckoutController(AppSettings settings, IOrderCheckoutCommand command, IMockShippingService shipping) : base(settings) {
			_checkoutCommand = command;
			_shippingService = shipping;
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

		[HttpPut, Route("{orderID}/shipping-quote")]
		public async Task<Order> SetShippingAndTax(string orderID, [FromBody] ShippingSelection shippingSelection)
		{
			return await _checkoutCommand.SetShippingSelection(orderID, shippingSelection);
		}
	}
}
