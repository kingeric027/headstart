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
	public class IntegrationController: BaseController
	{
		private readonly IOCShippingIntegration _OCShippingIntegration;
		public IntegrationController(AppSettings settings, IOCShippingIntegration OCShippingIntegration) : base(settings) 
		{
			_OCShippingIntegration = OCShippingIntegration;
		}

		// todo auth on this endpoint
		[Route("shippingrates")]
		[HttpPost]
		[OrderCloudWebhookAuth]
		public async Task<ProposedShipmentResponse> GetShippingRates([FromBody] OrderCalculationRequest orderCalculationRequest)
		{
			var proposedShipmentOptions = await _OCShippingIntegration.GetRatesAsync(orderCalculationRequest.OrderCalculation);
			return proposedShipmentOptions;
		}

		// todo auth on this endpoint
		[Route("ordercalculate")]
		[HttpPost]
		[OrderCloudWebhookAuth]
		public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculationRequest orderCalculationRequest)
		{
			var orderCalculationResponse = await _OCShippingIntegration.CalculateOrder(orderCalculationRequest.OrderCalculation);
			return orderCalculationResponse;
		}
	}
}
