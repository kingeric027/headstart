using Marketplace.Common.Services.FreightPop;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class ProposedShipmentMapper
    {
        public static ProposedShipment Map(ProposedShipmentRequest obj)
        {
            return new ProposedShipment
            {
                ProposedShipmentItems = obj.ProposedShipmentItems,
                ProposedShipmentOptions = obj.RateResponseTask.Result.Data.Rates.Select(rate => ProposedShipmentOptionMapper.Map(rate)).ToList()
            };
        }
    }
}