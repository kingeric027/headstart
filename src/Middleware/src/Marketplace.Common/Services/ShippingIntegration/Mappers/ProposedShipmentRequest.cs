using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration
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