using OrderCloud.SDK;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.FreightPop.Models;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public static class ShipmentItemMapper
    {
        public static FreightPop.Models.ShipmentItem Map(LineItem obj)
        {
            var rateItem = new FreightPop.Models.ShipmentItem
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