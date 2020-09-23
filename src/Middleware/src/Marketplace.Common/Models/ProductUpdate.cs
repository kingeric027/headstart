using Cosmonaut.Attributes;
using Marketplace.Models;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    public interface IProductHistory<T> : ICosmosObject
    {
        ActionType Action { get; set; }
        DateTime DateLastUpdated { get; set; }
        T Resource { get; set; }
        [CosmosPartitionKey]
        string ResourceID { get; set; }
    }

    [SwaggerModel]
    [CosmosCollection("productupdates")]
    public class ProductHistory : CosmosObject, IProductHistory<Product>
    {
        [CosmosPartitionKey]
        public string ResourceID { get; set; }
        public ActionType Action { get; set; }

        public Product Resource { get; set; }
        public DateTime DateLastUpdated { get; set; }

    }

    public class PriceScheduleHistory : CosmosObject, IProductHistory<PriceSchedule>
    {
        public ActionType Action { get; set; }
        public string ResourceID { get; set; }
        public PriceSchedule Resource { get; set; }
        public DateTime DateLastUpdated { get; set; }

    }

    public class ProductAssignmentHistory : CosmosObject, IProductHistory<ProductAssignment>
    {
        public ActionType Action { get; set; }
        public string ResourceID { get; set; }
        public ProductAssignment Resource { get; set; }
        public DateTime DateLastUpdated { get; set; }

    }

    public class ProductUpdateData
    {
        public string Supplier { get; set; }
        public DateTime TimeOfUpdate { get; set; }
        public string ProductID { get; set; }
        public ActionType Action { get; set; }
        public string OldProductType { get; set; }
        public string OldUnitMeasure { get; set; }
        public Nullable<int> OldUnitQty { get; set; }
        public bool? OldActiveStatus { get; set; }
        public string NewProductType { get; set; }
        public string NewUnitMeasure { get; set; }
        public Nullable<int> NewUnitQty { get; set; }
        public bool? NewActiveStatus { get; set; }
        public string PriceScheduleID { get; set; }
        public string Buyer { get; set; }
        public int OldMinQty { get; set; }
        public int OldMaxQty { get; set; }
        public List<PriceBreak> OldPriceBreak { get; set; }
        public int NewMinQty { get; set; }
        public int NewMaxQty { get; set; }
        public List<PriceBreak> NewPriceBreak { get; set; }

    }

    public enum ActionType
    {
        CreateProduct, 
        CreatePriceSchedule, 
        UpdateProduct, 
        UpdatePriceSchedule, 
        CreateProductPriceScheduleAssignments, 
        UpdateProductPriceScheduleAssignments
    }

}
