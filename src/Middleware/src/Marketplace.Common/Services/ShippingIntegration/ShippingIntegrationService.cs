using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers;
using Marketplace.Models;
using Marketplace.Models.Exceptions;
using OrderCloud.SDK;
using static Marketplace.Models.ErrorCodes;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IOCShippingIntegration
    {
        Task<List<ProposedShipment>> GetRatesAsync(OrderCalculation orderCalculation);
    }

    public class OCShippingIntegration : IOCShippingIntegration
    {
        readonly IFreightPopService _freightPopService;
        public OCShippingIntegration(IFreightPopService freightPopService)
        {
            _freightPopService = freightPopService;
        }

        public async Task<List<ProposedShipment>> GetRatesAsync(OrderCalculation orderCalculation)
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

            return proposedShipmentRequests.Select(proposedShipmentRequest => ProposedShipmentMapper.Map(proposedShipmentRequest)).ToList();
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

