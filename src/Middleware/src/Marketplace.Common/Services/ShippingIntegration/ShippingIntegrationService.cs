using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.Avalara;
using Marketplace.Common.Services.AvaTax;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers;
using Marketplace.Models.Exceptions;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using static Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IOCShippingIntegration
    {
        Task<ShipEstimateResponse> GetRatesAsync(OrderCalculatePayload orderCalculatePayload);
        Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload orderCalculatePayload);
    }

    public class OCShippingIntegration : IOCShippingIntegration
    {
        readonly IFreightPopService _freightPopService;
        private readonly IAvataxService _avatax;
        public OCShippingIntegration(IFreightPopService freightPopService, IAvataxService avatax)
        {
            _freightPopService = freightPopService;
            _avatax = avatax;
        }

        public async Task<ShipEstimateResponse> GetRatesAsync(OrderCalculatePayload orderCalculatePayload)
        {
            var orderWorksheet = orderCalculatePayload.OrderWorksheet;
            var productIDsWithInvalidDimensions = GetProductsWithInvalidDimensions(orderWorksheet.LineItems);
            Require.That(productIDsWithInvalidDimensions.Count == 0, Checkout.MissingProductDimensions, new MissingProductDimensionsError(productIDsWithInvalidDimensions));

            var proposedShipmentRequests = ShipmentEstimateRequestsMapper.Map(orderWorksheet);
            proposedShipmentRequests = proposedShipmentRequests.Select(proposedShipmentRequest =>
            {
                proposedShipmentRequest.RateResponseTask = _freightPopService.GetRatesAsync(proposedShipmentRequest.RateRequestBody);
                return proposedShipmentRequest;
            }).ToList();

            var tasks = proposedShipmentRequests.Select(p => p.RateResponseTask);
            await Task.WhenAll(tasks);

            var shipEstimates = proposedShipmentRequests.Select(proposedShipmentRequest => ShipmentEstimateMapper.Map(proposedShipmentRequest)).ToList();
            return new ShipEstimateResponse()
            {
                ShipEstimates = shipEstimates
            };
        }

        public async Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload orderCalculatePayload)
        {
            var totalTax = await _avatax.GetEstimateAsync(orderCalculatePayload.OrderWorksheet);

            return new OrderCalculateResponse
            {
                TaxTotal = totalTax,
            };

        }

        private List<string> GetProductsWithInvalidDimensions(IList<MarketplaceLineItem> lineItems)
        {
            return lineItems.Where(lineItem =>
            {
                return !(lineItem.Product.ShipHeight > 0 &&
                lineItem.Product.ShipLength > 0 &&
                lineItem.Product.ShipWeight > 0 &&
                lineItem.Product.ShipWidth > 0);
            }).Select(lineItem => lineItem.Product.ID).ToList();
        }
    }
}

