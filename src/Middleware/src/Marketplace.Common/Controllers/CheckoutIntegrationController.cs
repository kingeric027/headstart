using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;
using ordercloud.integrations.library;
using Marketplace.Common.Commands;

namespace Marketplace.Common.Controllers
{
	public class CheckoutIntegrationController: BaseController
	{
		private readonly ICheckoutIntegrationCommand _checkoutIntegrationCommand;
		private readonly IPostSubmitCommand _postSubmitCommand;
		public CheckoutIntegrationController(AppSettings settings, ICheckoutIntegrationCommand checkoutIntegrationCommand, IPostSubmitCommand postSubmitCommand) : base(settings) 
		{
			_checkoutIntegrationCommand = checkoutIntegrationCommand;
			_postSubmitCommand = postSubmitCommand;
		}

		[Route("shippingrates")]
		[HttpPost]
		[OrderCloudWebhookAuth]
		public async Task<ShipEstimateResponse> GetShippingRates([FromBody] MarketplaceOrderCalculatePayload orderCalculatePayload)
		{
			var shipmentEstimates = await _checkoutIntegrationCommand.GetRatesAsync(orderCalculatePayload);
			return shipmentEstimates;
		}

		[Route("ordercalculate")]
		[HttpPost]
		[OrderCloudWebhookAuth]
		public async Task<OrderCalculateResponse> CalculateOrder([FromBody] MarketplaceOrderCalculatePayload orderCalculatePayload)
		{
			var orderCalculationResponse = await _checkoutIntegrationCommand.CalculateOrder(orderCalculatePayload);
			return orderCalculationResponse;
		}

		[HttpPost, Route("ordersubmit")]
		[OrderCloudWebhookAuth]
		public async Task<OrderSubmitResponse> HandleOrderSubmit([FromBody] MarketplaceOrderCalculatePayload payload)
		{
			var response = await _postSubmitCommand.HandleBuyerOrderSubmit(payload.OrderWorksheet);
			return response;
		}

        [HttpPost, Route("ordersubmit/retry/zoho/{orderID}"), OrderCloudIntegrationsAuth(ApiRole.IntegrationEventAdmin)]
        public async Task<OrderSubmitResponse> RetryOrderSubmit(string orderID)
        {
            var retry = await _postSubmitCommand.HandleZohoRetry(orderID, this.VerifiedUserContext);
            return retry;
        }

        [HttpPost, Route("orderapproved")]
		[OrderCloudWebhookAuth]
		public async Task<OrderSubmitResponse> HandleOrderApproved([FromBody] MarketplaceOrderCalculatePayload payload)
		{
			var response = await _postSubmitCommand.HandleBuyerOrderSubmit(payload.OrderWorksheet);
			return response;
		}
	}
}
