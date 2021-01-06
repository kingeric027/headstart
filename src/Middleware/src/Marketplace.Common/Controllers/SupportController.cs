using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Models.Misc;
using Marketplace.Common.Services;
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
        private static IZohoCommand _zoho;
        private readonly ISupportAlertService _supportAlertService;
        private readonly IOrderCloudClient _oc;

        public SupportController(AppSettings settings, ICheckoutIntegrationCommand checkoutIntegrationCommand, IPostSubmitCommand postSubmitCommand, IZohoCommand zoho, IOrderCloudClient oc, ISupportAlertService supportAlertService) : base(settings)
        {
            _checkoutIntegrationCommand = checkoutIntegrationCommand;
            _postSubmitCommand = postSubmitCommand;
            _zoho = zoho;
            _oc = oc;
            _supportAlertService = supportAlertService;
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
        [HttpGet, OrderCloudIntegrationsAuth(ApiRole.IntegrationEventAdmin)]
        public async Task<OrderCalculateResponse> CalculateOrder(string orderID)
        {
            var orderCalculationResponse = await _checkoutIntegrationCommand.CalculateOrder(orderID, this.VerifiedUserContext);
            return orderCalculationResponse;
        }

        [HttpGet, Route("zoho/{orderID}")]
        public async Task<OrderSubmitResponse> RetryOrderSubmit(string orderID)
        {
            var retry = await _postSubmitCommand.HandleZohoRetry(orderID);
            return retry;
        }

        [HttpGet, Route("shipping/validate/{orderID}"), OrderCloudIntegrationsAuth(ApiRole.IntegrationEventAdmin)]
        public async Task<OrderSubmitResponse> RetryShippingValidate(string orderID)
        {
            var retry = await _postSubmitCommand.HandleShippingValidate(orderID, this.VerifiedUserContext);
            return retry;
        }

        // good debug method for testing rates with orders
        [HttpGet, Route("shippingrates/{orderID}")]
        public async Task<ShipEstimateResponse> GetShippingRates(string orderID)
        {
            return await _checkoutIntegrationCommand.GetRatesAsync(orderID);
        }

        [HttpPost, Route("postordersubmit/{orderID}"), OrderCloudIntegrationsAuth]
        public async Task<OrderSubmitResponse> ManuallyRunPostOrderSubmit(string orderID)
        {
            var worksheet = await _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderID);
            return await _postSubmitCommand.HandleBuyerOrderSubmit(worksheet);
        }

        [HttpPost, Route("submitcase")]
        public async Task SendSupportRequest([FromForm]SupportCase supportCase)
        {
            await _supportAlertService.EmailGeneralSupportQueue(supportCase);
        }
    }

    public class ShipmentTestModel
    {
        public MarketplaceOrder Order { get; set; }
        public List<MarketplaceLineItem> LineItems { get; set; }
    }
}
