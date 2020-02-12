using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.FreightPop.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class RateRequestBodyMapper
    {
        public static RateRequestBody Map(List<LineItem> obj)
        {
            var firstLineItem = obj[0];
            var shipToAddress = firstLineItem.ShippingAddress;
            var shipFromAddress = firstLineItem.ShipFromAddress;

            return new RateRequestBody
            {
                ConsigneeAddress = RateAddressMapper.Map(shipToAddress),
                ShipperAddress = RateAddressMapper.Map(shipFromAddress),
                Items = obj.Select(lineItem =>
                {
                    return RateItemMapper.Map(lineItem);
                }).ToList()
            };
        }
    }
}