using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [DocIgnore]
    [Route("support")]
    public class SupportController : BaseController
    {
        private static ICheckoutIntegrationCommand _checkoutIntegrationCommand;
        private static IPostSubmitCommand _postSubmitCommand;
        public SupportController(AppSettings settings, ICheckoutIntegrationCommand checkoutIntegrationCommand, IPostSubmitCommand postSubmitCommand) : base(settings)
        {
            _checkoutIntegrationCommand = checkoutIntegrationCommand;
            _postSubmitCommand = postSubmitCommand;
        }

        [HttpGet, Route("shipping")]
        public async Task<ShipEstimateResponse> GetShippingRates([FromBody] ShipmentTestModel model)
        {
            var payload = new MarketplaceOrderCalculatePayload()
            {
                ConfigData = null,
                OrderWorksheet = new MarketplaceOrderWorksheet()
                {
                    Order = model.Order,
                    LineItems = model.LineItems
                }
            };
            return await _checkoutIntegrationCommand.GetRatesAsync(payload);
        }

        [Route("tax/{orderID}")]
        [HttpPost, OrderCloudIntegrationsAuth(ApiRole.IntegrationEventAdmin)]
        public async Task<OrderCalculateResponse> CalculateOrder(string orderID)
        {
            var orderCalculationResponse = await _checkoutIntegrationCommand.CalculateOrder(orderID, this.VerifiedUserContext);
            return orderCalculationResponse;
        }

        [HttpPost, Route("zoho/{orderID}")]
        public async Task<OrderSubmitResponse> RetryOrderSubmit(string orderID)
        {
            var retry = await _postSubmitCommand.HandleZohoRetry(orderID);
            return retry;
        }

        [HttpPost, Route("shipping/validate/{orderID}"), OrderCloudIntegrationsAuth(ApiRole.IntegrationEventAdmin)]
        public async Task<OrderSubmitResponse> RetryShippingValidate(string orderID)
        {
            var retry = await _postSubmitCommand.HandleShippingValidate(orderID, this.VerifiedUserContext);
            return retry;
        }

        // good debug method for testing rates with orders
        [Route("shippingrates/{orderID}"), HttpGet]
        public async Task<ShipEstimateResponse> GetShippingRates(string orderID)
        {
            return await _checkoutIntegrationCommand.GetRatesAsync(orderID);
        }
    }

    public class ShipmentTestModel
    {
        public MarketplaceOrder Order { get; set; }
        public List<MarketplaceLineItem> LineItems { get; set; }
    }
}
