using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ShipmentEstimateItemMapper
    {
        public static ShipEstimateItem Map(LineItem obj)
        {
            return new ShipEstimateItem
            {
                LineItemID = obj.ID,
                Quantity = obj.Quantity
            };
        }
    }
}