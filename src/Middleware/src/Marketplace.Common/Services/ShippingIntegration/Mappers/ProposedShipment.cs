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
                ProposedShipmentItems = obj.ProposedShipmentItems,
                ProposedShipmentOptions = obj.RateResponseTask.Result.Data.Rates.Select(rate => ProposedShipmentOptionMapper.Map(rate)).ToList()
            };
        }
    }
}