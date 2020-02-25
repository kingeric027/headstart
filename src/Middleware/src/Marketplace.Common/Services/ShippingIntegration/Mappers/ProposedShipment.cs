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
                ProposedShipmentOptions = obj.RateResponseTask.Result.Data.Rates.Select(rate => ProposedShipmentOptionMapper.Map(rate)).ToList()
            };
        }
    }
}