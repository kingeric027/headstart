using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.exchangerates;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateMapper
    {
        public static ShipEstimate Map(ShipmentEstimateRequest obj, CurrencySymbol orderCurrency, List<OrderCloudIntegrationsConversionRate> rates)
        {
            return new ShipEstimate
            {
                ID = obj.ID,
                ShipEstimateItems = obj.ShipEstimateItems,
                ShipMethods = ShipmentEstimateMethodsMapper.Map(obj.RateResponseTask.Result.Data.Rates, orderCurrency, rates)
            };
        }
    }
}