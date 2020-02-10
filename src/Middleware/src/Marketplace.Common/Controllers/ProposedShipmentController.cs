using Marketplace.Common.Commands;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Extended;

namespace Marketplace.Common.Controllers
{
	[Route("shippingrates")]
	public class ProposedShipmentController: BaseController
	{
		private readonly IProposedShipmentCommand _shippingCommand;
		private readonly IOCShippingIntegration _OCShippingIntegration;
		public ProposedShipmentController(AppSettings settings, IProposedShipmentCommand shippingCommand, IOCShippingIntegration OCShippingIntegration) : base(settings) 
		{
			_shippingCommand = shippingCommand;
			_OCShippingIntegration = OCShippingIntegration;
		}

		//// investigate why ApiRole.Shopper auth is failing for shoppers
		//[HttpGet, Route("{orderId}"), MarketplaceUserAuth(ApiRole.SupplierReader)]
		//public async Task<MarketplaceListPage<ProposedShipment>> List(string orderId)
		//{
		//	var shippingRateResponse = await _shippingCommand.ListProposedShipments(orderId, VerifiedUserContext);
		//	return shippingRateResponse;
		//}

		[HttpPut, Route("{orderId}/select"), MarketplaceUserAuth(ApiRole.SupplierReader)]
		public async Task<MarketplaceOrder> Select(string orderId, [FromBody] ProposedShipmentSelection proposedShipmentSelection)
		{
			var marketplaceOrder = await _shippingCommand.SetShippingSelectionAsync(orderId, proposedShipmentSelection);
			return marketplaceOrder;
		}
		
		// todo auth on this endpoint
		[HttpPost]
		public async Task<List<ProposedShipment>> GetShippingRates([FromBody] OrderCalculationRequest orderCalculationRequest)
		{
			var proposedShipmentOptions = await _OCShippingIntegration.GetRates(orderCalculationRequest.OrderCalculation);
			return proposedShipmentOptions;
		}
	}
}
