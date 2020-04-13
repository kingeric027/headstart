using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateRequestMapper
    {
        public static ShipmentEstimateRequest Map(List<LineItem> obj)
        {
    
                var shipmentEstimateItems = obj.Select(lineItem => ShipmentEstimateItemMapper.Map(lineItem)).ToList();
                return new ShipmentEstimateRequest
                {
                    ID = obj[0].ShipFromAddressID,
                    ShipEstimateItems = shipmentEstimateItems,
                    RateRequestBody = RateRequestBodyMapper.Map(obj),
                };
        }
    }
}