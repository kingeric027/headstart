using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateRequestMapper
    {
        public static ShipmentEstimateRequest Map(List<MarketplaceLineItem> obj)
        {
    
                var shipmentEstimateItems = obj.Select(lineItem => ShipmentEstimateItemMapper.Map(lineItem)).ToList();
                return new ShipmentEstimateRequest
                {
                    ID = obj[0].ShipFromAddressID,
                    ShipmentEstimateItems = shipmentEstimateItems,
                    RateRequestBody = RateRequestBodyMapper.Map(obj),
                };
        }
    }
}