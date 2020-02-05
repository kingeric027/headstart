using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ProposedShipmentItemMapper
    {
        public static ProposedShipmentItem Map(LineItem obj)
        {
            return new ProposedShipmentItem
            {
                LineItemID = obj.ID,
                Quantity = obj.Quantity
            };
        }
    }
}