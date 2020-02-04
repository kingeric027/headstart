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
        public static Product Map(MarketplaceProduct m)
        {
            var oc = new Product()
            {
                Active = m.Active,
                AutoForward = m.AutoForward,
                ID = m.ID,
                DefaultPriceScheduleID = m.DefaultPriceScheduleID,
                DefaultSupplierID = m.DefaultSupplierID,
                Description = m.Description,
                Name = m.Name,
                QuantityMultiplier = m.QuantityMultiplier,
                ShipHeight = m.ShipHeight,
                ShipLength = m.ShipLength,
                ShipWeight = m.ShipWeight,
                ShipWidth = m.ShipWidth,
                xp = new
                {
                    m.Status,
                    m.Note,
                    m.UnitOfMeasure,
                    m.Images,
                    m.HasVariants,
                    m.Facets,
                    m.Tax
                }
            };
            return oc;
        }

        public static MarketplaceProduct Map(Product oc)
        {
            var m = new MarketplaceProduct()
            {
                Active = oc.Active,
                AutoForward = oc.AutoForward,
                ID = oc.ID,
                DefaultPriceScheduleID = oc.DefaultPriceScheduleID,
                DefaultSupplierID = oc.DefaultSupplierID,
                Description = oc.Description,
                Name = oc.Name,
                QuantityMultiplier = oc.QuantityMultiplier,
                ShipHeight = oc.ShipHeight,
                ShipLength = oc.ShipLength,
                ShipWeight = oc.ShipWeight,
                ShipWidth = oc.ShipWidth,
                Status = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "Status") : null,
                Note = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "Note") : null,
                UnitOfMeasure = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "UnitOfMeasure") : null,
                IntegrationData = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "Data") : null,
                Facets = oc.xp != null ? MapperHelper.TryFacetXp(oc.xp) : null,
                Images = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "Images") : null,
                Tax = oc.xp != null ? MapperHelper.TryGetXp(oc.xp, "Tax") : null
            };

            return m;
        }

        public static PartialProduct Map(Partial<MarketplaceProduct> m)
        {
            return m.Values.ToObject<PartialProduct>();
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
