using Marketplace.Common.Services.FreightPop;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class ProposedShipmentOptionMapper
    {
        public static ProposedShipmentOption Map(ShippingRate obj)
        {
            return new ProposedShipmentOption
            {
                ID = obj.Id,
                Name = obj.Service,
                DeliveryDays = obj.DeliveryDays,
                Cost = (decimal)obj.TotalCost
            };
        }
    }
}