using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Models.Attributes;
using Marketplace.Models.Extended;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.cms;
using ordercloud.integrations.easypost;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class SuperMarketplaceProduct : IMarketplaceObject
    {
        public string ID { get; set; }
        public MarketplaceProduct Product { get; set; }
        public PriceSchedule PriceSchedule { get; set; }
        public IList<Spec> Specs { get; set; }
        public IList<MarketplaceVariant> Variants { get; set; }
        public IList<Asset> Images { get; set; }
        public IList<Asset> Attachments { get; set; }
    }

    [SwaggerModel]
    public class SuperMarketplaceMeProduct : IMarketplaceObject
    {
        public string ID { get; set; }
        public MarketplaceMeProduct Product { get; set; }
        public PriceSchedule PriceSchedule { get; set; }
        public IList<Spec> Specs { get; set; }
        public IList<MarketplaceVariant> Variants { get; set; }
        public IList<Asset> Images { get; set; }
        public IList<Asset> Attachments { get; set; }
    }


    [SwaggerModel]
    public class PartialMarketplaceProduct : PartialProduct<ProductXp>
    {
    }
    [SwaggerModel]
    public class MarketplaceLineItemProduct : LineItemProduct<ProductXp> { }
    [SwaggerModel]
    public class MarketplaceProduct : Product<ProductXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class MarketplaceMeProduct: BuyerProduct<ProductXp, MarketplacePriceSchedule>
    {

    }

    [SwaggerModel]
	public class MarketplaceVariant : Variant<MarketplaceVariantXp> { }

    [SwaggerModel]
	public class ProductXp
    {
        #region DO NOT DELETE
        [OrchestrationIgnore]
        public dynamic IntegrationData { get; set; }
        public Dictionary<string, List<string>> Facets = new Dictionary<string, List<string>>();
        #endregion

        [System.ComponentModel.DataAnnotations.Required]
        public ObjectStatus? Status { get; set; }
        public bool HasVariants { get; set; }
        [MaxLength(500), OrchestrationIgnore]
        public string Note { get; set; }
        public TaxProperties Tax { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; } = new UnitOfMeasure();
        public ProductType ProductType { get; set; }
        public SizeTier SizeTier { get; set; }
        public bool IsResale { get; set; } = false;
        public List<ProductAccessorial> Accessorials { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public CurrencySymbol? Currency { get; set; } = null;
        public bool? ArtworkRequired { get; set; } = false;
        public bool PromotionEligible { get; set; }
        public bool FreeShipping { get; set; }
    }

	[JsonConverter(typeof(StringEnumConverter))]
	public enum ProductType
	{
		Standard,
		Quote,
		PurchaseOrder,
        Kit
	}

    [SwaggerModel]
    public class MarketplaceVariantXp
    {
        public string SpecCombo { get; set; }
        public List<SpecValue> SpecValues { get; set; }
        public string NewID { get; set; }
    }

    [SwaggerModel]
    public class SpecValue
    {
        public string SpecName { get; set; }
        public string SpecOptionValue { get; set; }
        public string PriceMarkup { get; set; }
    }

    public enum ProductAccessorial
    {
        Freezable = 6,
        Hazmat = 7,
        KeepFromFreezing = 8,
    }
}
