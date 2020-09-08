using Marketplace.Common.Services.ShippingIntegration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;
using ordercloud.integrations.library;
using Marketplace.Common.Commands;

namespace Marketplace.Common.Controllers
{
	public class IntegrationController: BaseController
	{
		private readonly IOCShippingIntegration _OCShippingIntegration;
		private readonly IPostSubmitCommand _postSubmitCommand;
		public IntegrationController(AppSettings settings, IOCShippingIntegration OCShippingIntegration, IPostSubmitCommand postSubmitCommand) : base(settings) 
		{
			_OCShippingIntegration = OCShippingIntegration;
			_postSubmitCommand = postSubmitCommand;
		}

		[Route("shippingrates")]
		[HttpPost]
		[OrderCloudWebhookAuth]
		public async Task<ShipEstimateResponse> GetShippingRates([FromBody] OrderCalculatePayload orderCalculatePayload)
		{
			var shipmentEstimates = await _OCShippingIntegration.GetRatesAsync(orderCalculatePayload);
			return shipmentEstimates;
		}

		[Route("ordercalculate")]
		[HttpPost]
		[OrderCloudWebhookAuth]
		public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculatePayload<MarketplaceOrderWorksheet> orderCalculatePayload)
		{
			var orderCalculationResponse = await _OCShippingIntegration.CalculateOrder(orderCalculatePayload);
			return orderCalculationResponse;

			//	given a free shipping cutoff value (supplier.xp.FreeShippingCutoff) set shipping cost to 0.
			//	Need to keep track of monetary value that was removed and what the threshold was (save it somewhere)
		}

		[HttpPost, Route("ordersubmit")]
		[OrderCloudWebhookAuth]
		public async Task<OrderSubmitResponse> HandleOrderSubmit([FromBody] OrderCalculatePayload<MarketplaceOrderWorksheet> payload)
		{
			var response = await _postSubmitCommand.HandleBuyerOrderSubmit(payload.OrderWorksheet);
			return response;
		}

		[HttpPost, Route("orderapproved")]
		[OrderCloudWebhookAuth]
		public async Task<OrderSubmitResponse> HandleOrderApproved([FromBody] OrderCalculatePayload<MarketplaceOrderWorksheet> payload)
		{
			var response = await _postSubmitCommand.HandleBuyerOrderSubmit(payload.OrderWorksheet);
			return response;
		}
	}
}
