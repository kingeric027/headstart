using OrderCloud.SDK;
using ordercloud.integrations.freightpop;
using Marketplace.Models.Models.Marketplace;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class ShipmentItemMapper
    {
        public static ordercloud.integrations.freightpop.ShipmentItem Map(MarketplaceLineItem obj)
        {
            var rateItem = new ordercloud.integrations.freightpop.ShipmentItem
            {
                // todo consider inner pieces
                //InnerPieces =

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