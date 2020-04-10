using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateMapper
    {
        public static ShipEstimate Map(ShipmentEstimateRequest obj)
        {
            return new ShipEstimate
            {
                ID = obj.ID,
                ShipEstimateItems = obj.ShipEstimateItems,
                ShipMethods = ShipmentEstimateMethodsMapper.Map(obj.RateResponseTask.Result.Data.Rates)
            };
        }
    }
}