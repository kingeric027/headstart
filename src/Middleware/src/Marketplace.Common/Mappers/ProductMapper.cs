using Marketplace.Models;
using Marketplace.Models.Extended;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Mappers
{
   public class ProductMapper
   {
        public static MarketplaceProduct MeProductToProduct(BuyerProduct<ProductXp, PriceSchedule> product)
        {
            return new MarketplaceProduct
            {
                Active = product.Active,
                AutoForward = false,
                DefaultPriceScheduleID = product.PriceSchedule.ID,
                DefaultSupplierID = null,
                Description = product.Description,
                ID = product.ID,
                Inventory = product.Inventory,
                Name = product.Name,
                OwnerID = null,
                QuantityMultiplier = product.QuantityMultiplier,
                ShipFromAddressID = product.ShipFromAddressID,
                ShipHeight = product.ShipHeight,
                ShipLength = product.ShipLength,
                ShipWeight = product.ShipWeight,
                ShipWidth = product.ShipWidth,
                SpecCount = product.SpecCount,
                VariantCount = product.VariantCount,
                xp = product.xp
            };
        }
    }
}
