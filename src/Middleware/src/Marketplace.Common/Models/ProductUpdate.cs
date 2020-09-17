using Cosmonaut.Attributes;
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
    public class ProductUpdate : CosmosObject
    {
        [CosmosInteropID]
        public string ProductID { get; set; }
        [CosmosPartitionKey]
        public string SupplierID { get; set; }
        public Product ProductBeforeUpdate { get; set; }
        public Product ProductAfterUpdate { get; set; }
    }
}
