using System.Collections;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class ProposedShipmentMapper
    {
        public static ProposedShipment Map(ProposedShipmentRequest obj)
        {
            return new ProposedShipment
            {
                ID = obj.ID,
                ProposedShipmentItems = obj.ProposedShipmentItems,
                ProposedShipmentOptions = ProposedShipmentOptionsMapper.Map(obj.RateResponseTask.Result.Data.Rates)
            };
        }
    }
}