using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.Avalara;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Exceptions;
using ordercloud.integrations.extensions;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;
using static Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IOCShippingIntegration
    {
        Task<ShipEstimateResponse> GetRatesAsync(OrderCalculatePayload orderCalculatePayload);
        Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload<MarketplaceOrderWorksheet> orderCalculatePayload);
    }

    public class OCShippingIntegration : IOCShippingIntegration
    {
        readonly IFreightPopService _freightPopService;
        private readonly IAvalaraCommand _avatax;
        public OCShippingIntegration(IFreightPopService freightPopService, IAvalaraCommand avatax)
        {
            _freightPopService = freightPopService;
            _avatax = avatax;
        }

        public async Task<ShipEstimateResponse> GetRatesAsync(OrderCalculatePayload orderCalculatePayload)
        {
            var orderWorksheet = orderCalculatePayload.OrderWorksheet;
            var lineItemsForShippingEstimates = GetLineItemsToIncludeInShipping(orderCalculatePayload.OrderWorksheet.LineItems, orderCalculatePayload.ConfigData);
            var productIDsWithInvalidDimensions = GetProductsWithInvalidDimensions(lineItemsForShippingEstimates);
            Require.That(productIDsWithInvalidDimensions.Count == 0, Checkout.MissingProductDimensions, new MissingProductDimensionsError(productIDsWithInvalidDimensions));

            var proposedShipmentRequests = ShipmentEstimateRequestsMapper.Map(lineItemsForShippingEstimates);
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
                ShipEstimates = shipEstimates as IList<ShipEstimate>
            };
        }

        private List<LineItem> GetLineItemsToIncludeInShipping(IList<LineItem> lineItems, CheckoutIntegrationConfiguration configData)
        {
            if(configData.ExcludePOProductsFromShipping)
            {
                return lineItems.Where(li =>
                {
                    return !(li.Product.xp.ProductType == ProductType.PurchaseOrder.ToString());
                }).ToList();
            } else {
                return lineItems.ToList();
            }
        }

        public async Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload<MarketplaceOrderWorksheet> orderCalculatePayload)
        {
            if(orderCalculatePayload.OrderWorksheet.Order.xp != null && orderCalculatePayload.OrderWorksheet.Order.xp.OrderType == Marketplace.Models.Extended.OrderType.Quote)
            {
                // quote orders do not have tax cost associated with them
                return new OrderCalculateResponse();
            } else
            {
                var totalTax = await _avatax.GetEstimateAsync(orderCalculatePayload.OrderWorksheet);

                return new OrderCalculateResponse
                {
                    TaxTotal = totalTax,
                };
            }

        }

        private List<string> GetProductsWithInvalidDimensions(IList<LineItem> lineItems)
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

