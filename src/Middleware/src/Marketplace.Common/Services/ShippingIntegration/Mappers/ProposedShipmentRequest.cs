using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class ProposedShipmentRequestMapper
    {
        public static ProposedShipmentRequest Map(List<LineItem> obj)
        {
    
                var proposedShipmentItems = obj.Select(lineItem => ProposedShipmentItemMapper.Map(lineItem)).ToList();

                var firstLineItem = obj[0];
                var shipToAddress = firstLineItem.ShippingAddress;
                var shipFromAddress = firstLineItem.ShipFromAddress;

                return new ProposedShipmentRequest
                {
                    ProposedShipmentItems = proposedShipmentItems,
                    RateRequestBody = new RateRequestBody
                    {
                        ConsigneeAddress = RateAddressMapper.Map(shipToAddress),
                        ShipperAddress = RateAddressMapper.Map(shipFromAddress),
                        Items = obj.Select(lineItem =>
                        {
                            return RateItemMapper.Map(lineItem);
                        }).ToList()
                    }
                };
        }
    }
}