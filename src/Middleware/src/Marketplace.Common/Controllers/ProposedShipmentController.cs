using Avalara.AvaTax.RestClient;
using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
	[Route("proposedshipment")]
	public class ProposedShipmentController: BaseController
	{
		private readonly IProposedShipmentCommand _shippingCommand;
		public ProposedShipmentController(AppSettings settings, IProposedShipmentCommand shippingCommand) : base(settings) 
		{
			_shippingCommand = shippingCommand;
		}

		[HttpGet, Route("{orderId}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<MarketplaceListPage<ProposedShipment>> List(string orderId)
		{
			var shippingRateResponse = await _shippingCommand.ListProposedShipments(orderId, VerifiedUserContext);
			return shippingRateResponse;
		}
	}
}
