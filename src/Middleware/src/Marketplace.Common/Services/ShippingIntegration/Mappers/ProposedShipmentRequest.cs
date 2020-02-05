using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ProposedShipmentRequestMapper
    {
        public static ProposedShipmentRequest Map(List<LineItem> obj)
        {
    
                var proposedShipmentItems = obj.Select(lineItem => ProposedShipmentItemMapper.Map(lineItem)).ToList();
                return new ProposedShipmentRequest
                {
                    ProposedShipmentItems = proposedShipmentItems,
                    RateRequestBody = RateRequestBodyMapper.Map(obj),
                };
        }
    }
}