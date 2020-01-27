using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration
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