using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ProposedShipmentRequestMapper
    {
        public static ProposedShipmentRequest Map(List<MarketplaceLineItem> obj)
        {
    
                var proposedShipmentItems = obj.Select(lineItem => ProposedShipmentItemMapper.Map(lineItem)).ToList();
                return new ProposedShipmentRequest
                {
                    ID = obj[0].ShipFromAddressID,
                    ProposedShipmentItems = proposedShipmentItems,
                    RateRequestBody = RateRequestBodyMapper.Map(obj),
                };
        }
    }
}