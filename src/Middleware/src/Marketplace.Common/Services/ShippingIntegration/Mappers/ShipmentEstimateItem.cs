using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateItemMapper
    {
        public static ShipmentEstimateItem Map(LineItem obj)
        {
            return new ShipmentEstimateItem
            {
                LineItemID = obj.ID,
                Quantity = obj.Quantity
            };
        }
    }
}