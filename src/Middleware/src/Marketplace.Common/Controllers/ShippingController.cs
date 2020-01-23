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
	[Route("orders/{orderID}/shipping")]
	public class ShippingController: BaseController
	{
		private readonly IShippingCommand _shippingCommand;
		private readonly IMockShippingCacheService _shippingCache; 

		// Needs more authentication. These methods should only work for a specific user's orders.
		public ShippingController(AppSettings settings, IShippingCommand shippingCommand, IMockShippingCacheService shippingCache) : base(settings) {
			_shippingCommand = shippingCommand;
			_shippingCache = shippingCache;
		}

		[HttpPost, Route("generate-rates"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<IEnumerable<ShippingOptions>> GenerateShippingRatesAsync(string orderID)
		{
			return await _shippingCommand.GenerateShippingRatesAsync(orderID);
		}

		[HttpGet, Route("rate/{rateID}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<ShippingRate> GetSavedShippingRateAsync(string orderID, string rateID)
		{
			return await _shippingCache.GetSavedShippingRateAsync(orderID, rateID);
		}

		[HttpPut, Route("select"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, [FromBody] ShippingSelection shippingSelection)
		{
			return await _shippingCommand.SetShippingSelectionAsync(orderID, shippingSelection);
		}
	}
}
