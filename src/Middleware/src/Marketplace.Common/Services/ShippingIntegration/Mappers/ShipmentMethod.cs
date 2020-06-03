using ordercloud.integrations.freightpop;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateMethodsMapper
    {
        private static List<ShippingRate> GetCheapestRatesForEachDeliveryDays(List<ShippingRate> shippingRates)
        {
            var shippingRatesSortedByCost = shippingRates.OrderBy(rate => rate.ListCost);
            var cheapestRatesForEachDeliveryDays = shippingRatesSortedByCost.Where(sortedRate =>
            {
                var cheapestRateForDeliveryDays = shippingRatesSortedByCost.First(gettingFirstRate => gettingFirstRate.DeliveryDays == sortedRate.DeliveryDays);
                return cheapestRateForDeliveryDays.Id == sortedRate.Id;
            });
            return cheapestRatesForEachDeliveryDays.ToList();
        }
        
        public static List<ShipMethod> Map(IList<ShippingRate> shippingRates)
        {
            var cheapestRates = GetCheapestRatesForEachDeliveryDays(shippingRates.ToList());
            return cheapestRates.Select(cheapestRate =>
            {
                return ShipmentMethodMapper.Map(cheapestRate);
            }).ToList();
        }
    }

    public static class ShipmentMethodMapper
    {
        public static ShipMethod Map(ShippingRate obj)
        {
            return new ShipMethod
            {
                ID = obj.Id,
                Name = obj.Service,
                EstimatedTransitDays = obj.DeliveryDays,
                Cost = (decimal)obj.TotalCost
            };
        }
    }
}