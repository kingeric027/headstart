using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers;
using Marketplace.Models;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IOCShippingIntegration
    {
        Task<List<ProposedShipment>> GetProposedShipmentsForSuperOrderAsync(SuperOrder superOrder);
    }

    public class OCShippingIntegration : IOCShippingIntegration
    {
        readonly IFreightPopService _freightPopService;
        public OCShippingIntegration(IFreightPopService freightPopService)
        {
            _freightPopService = freightPopService;
        }

        public async Task<List<ProposedShipment>> GetProposedShipmentsForSuperOrderAsync(SuperOrder superOrder)
        {
            var productIDsWithInvalidDimensions = GetProductsWithInvalidDimensions(superOrder);
            Require.That(productIDsWithInvalidDimensions.Count == 0, ErrorCodes.Checkout.MissingProductDimensions, new MissingProductDimensionsError(productIDsWithInvalidDimensions));

            var proposedShipmentRequests = ProposedShipmentRequestsMapper.Map(superOrder);
            proposedShipmentRequests = proposedShipmentRequests.Select(proposedShipmentRequest =>
            {
                proposedShipmentRequest.RateResponseTask = _freightPopService.GetRatesAsync(proposedShipmentRequest.RateRequestBody);
                return proposedShipmentRequest;
            }).ToList();

            var tasks = proposedShipmentRequests.Select(p => p.RateResponseTask);
            await Task.WhenAll(tasks);

            return proposedShipmentRequests.Select(proposedShipmentRequest => ProposedShipmentMapper.Map(proposedShipmentRequest)).ToList();
        }

        private static List<string> GetProductsWithInvalidDimensions(SuperOrder superOrder)
        {
            return superOrder.LineItems.Where(lineItem => !(lineItem.Product.ShipHeight > 0 &&
                                                            lineItem.Product.ShipLength > 0 &&
                                                            lineItem.Product.ShipWeight > 0 &&
                                                            lineItem.Product.ShipWidth > 0)).Select(lineItem => lineItem.Product.ID).ToList();
        }
    }
}

