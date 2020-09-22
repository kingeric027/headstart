﻿using Cosmonaut.Attributes;
using Marketplace.Models;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
    [SwaggerModel]
    [CosmosCollection("productupdates")]
    public class ProductHistory : CosmosObject
    {
        [CosmosPartitionKey]
        public string ProductID { get; set; }
        public ActionType Action { get; set; }

        public Product Product { get; set; }
        public DateTime DateLastUpdated { get; set; }

    }

    public class PriceScheduleHistory : CosmosObject
    {
        public ActionType Action { get; set; }
        public string PriceScheduleID { get; set; }
        public MarketplacePriceSchedule PriceSchedule { get; set; }
        public DateTime DateLastUpdated { get; set; }

    }

    public class ProductAssignmentHistory : CosmosObject
    {
        public ActionType Action { get; set; }
        public string ProductID { get; set; }
        public ProductAssignment ProductAssignment { get; set; }
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
        public bool? OldActiveStatus { get; set; }
        public string NewProductType { get; set; }
        public string NewUnitMeasure { get; set; }
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
