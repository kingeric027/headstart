using ordercloud.integrations.freightpop;
using Marketplace.Common.Services.ShippingIntegration.Models;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class OCShipmentItemMapper
    {
        public static OrderCloud.SDK.ShipmentItem Map(ordercloud.integrations.freightpop.ShipmentItem freightPopShipmentItem, string ocOrderID)
        {
            return new OrderCloud.SDK.ShipmentItem
            {
                OrderID = ocOrderID,
                LineItemID = freightPopShipmentItem.PackageId,
                QuantityShipped = freightPopShipmentItem.Quantity,
            };
        }
    }
}