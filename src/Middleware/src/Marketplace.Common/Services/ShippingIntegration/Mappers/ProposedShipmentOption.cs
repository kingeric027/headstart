using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
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