using Marketplace.Common.Commands;
using Marketplace.Common.Models;
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
		private readonly IOrderCheckoutCommand _command;
		public OrderCheckoutController(IAppSettings settings, IOrderCheckoutCommand command) : base(settings) {
			_command = command;
		}

		[HttpGet, Route("{orderID}/shipping/quotes"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<IEnumerable<MockShippingQuote>> ListShippingQuotes(string orderID)
		{
			return await _command.ListShippingQuotes(orderID);
		}

		[HttpGet, Route("{orderID}/shipping/quotes/{quoteID}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MockShippingQuote> GetSingleShippingQuote(string orderID, string quoteID)
		{
			return await _command.GetSingleShippingQuote(orderID, quoteID);
		}

		[HttpPost, Route("{orderID}/shipping/quotes/{quoteID}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<Order> SetShippingQuoteAndCalculateTax(string orderID, string quoteID)
		{
			return await _command.SetShippingQuote(orderID, quoteID);
		}
	}
}
