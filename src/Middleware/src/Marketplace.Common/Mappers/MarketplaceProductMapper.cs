using System;
using System.Linq;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers
{
    public static class MarketplaceProductMapper
    {
        // pattern: <Target Object> Map(<Reference Object>)
        public static Product Map(MarketplaceProduct mProduct)
        {
            var ocProduct = new Product()
            {
                Active = mProduct.Active,
                AutoForward = mProduct.AutoForward,
                ID = mProduct.ID,
                DefaultPriceScheduleID = mProduct.DefaultPriceScheduleID,
                DefaultSupplierID = mProduct.DefaultSupplierID,
                Description = mProduct.Description,
                Name = mProduct.Name,
                QuantityMultiplier = mProduct.QuantityMultiplier,
                ShipHeight = mProduct.ShipHeight,
                ShipLength = mProduct.ShipLength,
                ShipWeight = mProduct.ShipWeight,
                ShipWidth = mProduct.ShipWidth,
                xp = new
                {
                    mProduct.Status,
                    mProduct.Note,
                    mProduct.UnitOfMeasure,
                    mProduct.Images,
                    mProduct.HasVariants
                }
            };
            return ocProduct;
        }

        public static MarketplaceProduct Map(Product ocProduct)
        {
            var mProduct = new MarketplaceProduct()
            {
                Active = ocProduct.Active,
                AutoForward = ocProduct.AutoForward,
                ID = ocProduct.ID,
                DefaultPriceScheduleID = ocProduct.DefaultPriceScheduleID,
                DefaultSupplierID = ocProduct.DefaultSupplierID,
                Description = ocProduct.Description,
                Name = ocProduct.Name,
                QuantityMultiplier = ocProduct.QuantityMultiplier,
                ShipHeight = ocProduct.ShipHeight,
                ShipLength = ocProduct.ShipLength,
                ShipWeight = ocProduct.ShipWeight,
                ShipWidth = ocProduct.ShipWidth,
                Status = MapperHelper.TryGetXp(ocProduct.xp, "Status"),
                Note = MapperHelper.TryGetXp(ocProduct.xp, "Note"),
                UnitOfMeasure = MapperHelper.TryGetXp(ocProduct.xp, "UnitOfMeasure"),
                IntegrationData = MapperHelper.TryGetXp(ocProduct.xp, "Data"),
                Facets = MapperHelper.TryFacetXp(ocProduct.xp),
                Images = MapperHelper.TryGetXp(ocProduct.xp, "Images")
            };

            return mProduct;
        }

        public static PartialProduct Map(Partial<MarketplaceProduct> mProduct)
        {
            return mProduct.Values.ToObject<PartialProduct>();
        }

        public static MarketplaceListPage<MarketplaceProduct> Map(ListPage<Product> ocProducts)
        {
            var list = new MarketplaceListPage<MarketplaceProduct>
            {
                Items = ocProducts.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(ocProducts.Meta)
            };
            return list;
        }
    }
}
