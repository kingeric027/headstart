using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Dynamitey;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Mappers;
using Marketplace.Helpers.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OrderCloud.SDK;

namespace Marketplace.Common.Mappers.Crud
{
    public static class MarketplaceProductMapper
    {
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
                    mProduct.UnitOfMeasure
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
                Note = MapperHelper.TryGetXp(ocProduct.xp, "Note"),
                UnitOfMeasure = MapperHelper.TryGetXp(ocProduct.xp, "UnitOfMeasure"),
                IntegrationData = MapperHelper.TryGetXp(ocProduct.xp, "Data")
            };
            
            return mProduct;
        }

        public static PartialProduct Map(Partial<MarketplaceProduct> mProduct)
        {
            return mProduct.Values.ToObject<PartialProduct>();
        }

        public static Marketplace.Helpers.Models.ListPage<MarketplaceProduct> Map(OrderCloud.SDK.ListPage<Product> ocProducts)
        {
            var list = new Marketplace.Helpers.Models.ListPage<MarketplaceProduct>
            {
                Items = ocProducts.Items.Select(Map).ToList(),
                Meta = ListPageMetaMapper.MapListFrom(ocProducts.Meta)
            };
            return list;
        }
    }
}
