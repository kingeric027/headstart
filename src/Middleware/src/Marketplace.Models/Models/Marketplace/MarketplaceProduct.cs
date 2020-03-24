using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marketplace.Models.Extended;
using OrderCloud.SDK;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Models
{
    public class SuperMarketplaceProduct : IMarketplaceObject
    {
        public MarketplaceProduct Product { get; set; }
        public PriceSchedule PriceSchedule { get; set; }
        public string ID { get; set; }
    }

    public class PartialMarketplaceProduct : PartialProduct<ProductXp>
    {
       public new ProductXp xp { get; set; } = new ProductXp();
    }

    public class MarketplaceProduct : Product<ProductXp>, IMarketplaceObject
    {
    }

    public class ProductXp
    {
        #region DO NOT DELETE
        [OrchestrationIgnore]
        public dynamic IntegrationData { get; set; }
        public Dictionary<string, List<string>> Facets = new Dictionary<string, List<string>>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        #endregion

        [Required]
        public ObjectStatus? Status { get; set; }
        public bool HasVariants { get; set; }
        [MaxLength(500), OrchestrationIgnore]
        public string Note { get; set; }
        public TaxProperties Tax { get; set; } = new TaxProperties();
        public UnitOfMeasure UnitOfMeasure { get; set; } = new UnitOfMeasure();
        public ProductType ProductType { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class OrchestrationIgnoreAttribute : Attribute { }
}
