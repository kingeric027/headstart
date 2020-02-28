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
        public IList<Spec> Specs { get; set; }
        public IList<Variant<MarketplaceVariantXp>> Variants { get; set; }
    }

    public class PartialMarketplaceProduct : PartialProduct<ProductXp>
    {
    }

    public class MarketplaceProduct : Product<ProductXp>, IMarketplaceObject
    {
    }

    public class ProductXp
    {
        #region DO NOT DELETE
        public dynamic IntegrationData { get; set; }
        public Dictionary<string, List<string>> Facets = new Dictionary<string, List<string>>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        #endregion

        [Required]
        public ObjectStatus? Status { get; set; }
        public bool HasVariants { get; set; }
        [MaxLength(500)]
        public string Note { get; set; }
        public TaxProperties Tax { get; set; } = new TaxProperties();
        public UnitOfMeasure UnitOfMeasure { get; set; } = new UnitOfMeasure();
        public ProductType ProductType { get; set; }
        public List<StaticContent> StaticContent { get; set; } = new List<StaticContent>();
    }

    public class MarketplaceVariantXp
    {
        public List<SpecValue> SpecValues { get; set; }
        public string NewID { get; set; }
    }

    public class SpecValue
    {
        public string SpecName { get; set; }
        public string SpecOptionValue { get; set; }
        public string PriceMarkup { get; set; }
    }
}
