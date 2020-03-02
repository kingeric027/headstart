using Marketplace.Common.Services.ShippingIntegration;
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
		public async Task<ProposedShipmentResponse> GetShippingRates([FromBody] OrderCalculationRequest orderCalculationRequest)
		{
			var proposedShipmentOptions = await _OCShippingIntegration.GetRatesAsync(orderCalculationRequest.OrderCalculation);
			return proposedShipmentOptions;
		}

		// todo auth on this endpoint
		[Route("ordercalculate")]
		[HttpPost]
		public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculationRequest orderCalculationRequest)
		{
			var orderCalculationResponse = await _OCShippingIntegration.CalculateOrder(orderCalculationRequest.OrderCalculation);
			return orderCalculationResponse;
		}
	}
}
