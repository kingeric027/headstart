using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateMapper
    {
        public static ShipmentEstimate Map(ShipmentEstimateRequest obj)
        {
            return new ShipmentEstimate
            {
                ID = obj.ID,
                ShipmentEstimateItems = obj.ShipmentEstimateItems,
                ShipmentMethods = ShipmentEstimateMethodsMapper.Map(obj.RateResponseTask.Result.Data.Rates)
            };
        }
    }
}