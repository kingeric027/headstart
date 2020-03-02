using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Models;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ProposedShipmentOptionsMapper
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
        
        public static List<ProposedShipmentOption> Map(IList<ShippingRate> shippingRates)
        {
            var cheapestRates = GetCheapestRatesForEachDeliveryDays(shippingRates.ToList());
            return cheapestRates.Select(cheapestRate =>
            {
                return ProposedShipmentOptionMapper.Map(cheapestRate);
            }).ToList();
        }
    }

    public static class ProposedShipmentOptionMapper
    {
        public static ProposedShipmentOption Map(ShippingRate obj)
        {
            return new ProposedShipmentOption
            {
                ID = obj.Id,
                Name = obj.Service,
                EstimatedDeliveryDays = obj.DeliveryDays,
                Cost = (decimal)obj.TotalCost
            };
        }
    }
}