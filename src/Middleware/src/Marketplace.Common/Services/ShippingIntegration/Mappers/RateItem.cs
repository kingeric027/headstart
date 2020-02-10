using Marketplace.Common.Services.FreightPop.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class RateItemMapper
    {
        public static Item Map(LineItem obj)
        {
            var rateItem = new Item
            {
                Description = obj.Product.Description,
                Height = obj.Product.ShipHeight ?? 0,
                Width = obj.Product.ShipWidth ?? 0,
                Length = obj.Product.ShipLength ?? 0,
                Weight = obj.Product.ShipWeight ?? 0,
                Quantity = obj.Quantity,
                PackageId = obj.ID,

                // standardizing this measurement for now
                Unit = Unit.lbs_inch,

                // hard coding these now, need to determine where this will come from
                // likely a change to the product model
                PackageType = PackageType.Box,

                // need to figure out how to get this correct
                FreightClass = 1
            };
            return rateItem;
        }
    }
}