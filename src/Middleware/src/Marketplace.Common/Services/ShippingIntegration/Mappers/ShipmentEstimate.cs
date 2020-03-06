using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateMapper
    {
        public static ShipEstimate Map(ShipmentEstimateRequest obj)
        {
            return new ShipEstimate
            {
                ID = obj.ID,
                ShipEstimateItems = obj.ShipmentEstimateItems,
                ShipMethods = ShipmentEstimateMethodsMapper.Map(obj.RateResponseTask.Result.Data.Rates)
            };
        }
    }
}