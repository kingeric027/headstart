using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marketplace.Models.Extended;
using OrderCloud.SDK;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Marketplace.Models
{
    public abstract class OrchestrationModel
    {
        public Dictionary<string, object> Props { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Get a property value by name.
        /// </summary>
        protected T GetProp<T>(string name) => Props.TryGetValue(name, out object value) ? (T)value : default(T);

        /// <summary>
        /// Get a property value by name, and provide a default value if the property hasn't been explicitly set.
        /// </summary>
        protected T GetProp<T>(string name, T defaultValue)
        {
            if (Props.TryGetValue(name, out object value))
                return (T)value;

            if (this is IPartial)
                return default(T);
            else
            {
                SetProp(name, defaultValue);
                return defaultValue;
            }
        }

        /// <summary>
        /// Set a property value by name.
        /// </summary>
        protected void SetProp<T>(string name, T value) => Props[name] = value;
    }

    public class SuperMarketplaceProduct : OrchestrationModel, IMarketplaceObject
    {
        public MarketplaceProduct Product { get; set; }
        public PriceSchedule PriceSchedule { get; set; }
        public string ID { get; set; }
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
        [ApiNoUpdate]
        public dynamic IntegrationData { get; set; }
        public Dictionary<string, List<string>> Facets = new Dictionary<string, List<string>>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        #endregion

        [Required]
        public ObjectStatus? Status { get; set; }
        public bool HasVariants { get; set; }
        [MaxLength(500), ApiNoUpdate]
        public string Note { get; set; }
        public TaxProperties Tax { get; set; } = new TaxProperties();
        public UnitOfMeasure UnitOfMeasure { get; set; } = new UnitOfMeasure();
        public ProductType ProductType { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ApiNoUpdateAttribute : Attribute { }
}
