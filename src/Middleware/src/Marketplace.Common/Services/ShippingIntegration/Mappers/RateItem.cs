using ordercloud.integrations.freightpop;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Mappers
{
    public static class RateItemMapper
    {
        // will be able to get rid of this when smarty streets address validation is done
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

        public static Item Map(MarketplaceLineItem obj)
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