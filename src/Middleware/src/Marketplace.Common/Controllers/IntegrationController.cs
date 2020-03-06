using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;

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
		public async Task<ShipEstimateResponse> GetShippingRates([FromBody] OrderCalculatePayload orderCalculatePayload)
		{
			var shipmentEstimates = await _OCShippingIntegration.GetRatesAsync(orderCalculatePayload);
			return shipmentEstimates;
		}

		// todo auth on this endpoint
		[Route("ordercalculate")]
		[HttpPost]
		[OrderCloudWebhookAuth]
		public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculatePayload orderCalculatePayload)
		{
			var orderCalculationResponse = await _OCShippingIntegration.CalculateOrder(orderCalculatePayload);
			return orderCalculationResponse;
		}
	}
}
