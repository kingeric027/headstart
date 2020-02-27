﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.AvaTax;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers;
using Marketplace.Models;
using Marketplace.Models.Exceptions;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using static Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IOCShippingIntegration
    {
        Task<ProposedShipmentResponse> GetRatesAsync(OrderCalculation orderCalculation);
        Task<OrderCalculateResponse> CalculateOrder(OrderCalculation orderCalculation);
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

        public async Task<ProposedShipmentResponse> GetRatesAsync(OrderCalculation orderCalculation)
        {
            var productIDsWithInvalidDimensions = GetProductsWithInvalidDimensions(orderCalculation.LineItems);
            Require.That(productIDsWithInvalidDimensions.Count == 0, Checkout.MissingProductDimensions, new MissingProductDimensionsError(productIDsWithInvalidDimensions));

            var proposedShipmentRequests = ProposedShipmentRequestsMapper.Map(orderCalculation);
            proposedShipmentRequests = proposedShipmentRequests.Select(proposedShipmentRequest =>
            {
                proposedShipmentRequest.RateResponseTask = _freightPopService.GetRatesAsync(proposedShipmentRequest.RateRequestBody);
                return proposedShipmentRequest;
            }).ToList();

            var tasks = proposedShipmentRequests.Select(p => p.RateResponseTask);
            await Task.WhenAll(tasks);

            var proposedShipments = proposedShipmentRequests.Select(proposedShipmentRequest => ProposedShipmentMapper.Map(proposedShipmentRequest)).ToList();
            return new ProposedShipmentResponse()
            {
                ProposedShipments = proposedShipments
            };
        }

        public async Task<OrderCalculateResponse> CalculateOrder(OrderCalculation orderCalculation)
        {
            var totalTax = await _avatax.GetTaxEstimateAsync(orderCalculation);
            var totalShippingCost = SumProposedShipmentCosts(orderCalculation);

            return new OrderCalculateResponse
            {
                TaxTotal = totalTax,
                ShippingTotal = totalShippingCost,
            };

        }

        private decimal SumProposedShipmentCosts(OrderCalculation orderCalculation)
        {
            var selectedProposedShipmentOptions = orderCalculation.ProposedShipmentRatesResponse.ProposedShipments.Select(proposedShipment =>
            {
                return proposedShipment.ProposedShipmentOptions
                .First(proposedShipmentOption => proposedShipmentOption.ID == proposedShipment.SelectedProposedShipmentOptionID).Cost;
            });
            return selectedProposedShipmentOptions.Sum();
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

