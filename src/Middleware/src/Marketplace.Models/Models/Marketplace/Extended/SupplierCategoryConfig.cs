using System;
using System.Collections.Generic;
using Cosmonaut.Attributes;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;

namespace Marketplace.Models.Extended
{
    [SwaggerModel]
	[CosmosCollection("suppliercategoryconfigs")]
	public class SupplierCategoryConfig : ICosmosObject
    {
        public SupplierCategoryConfig()
        {
        }

        public string id { get; set; }
        public DateTimeOffset timeStamp { get; set; }
        public string MarketplaceName { get; set; }
        public IEnumerable<SupplierCategoriesFilter> Filters { get; set; }
    }

    [SwaggerModel]
    public class SupplierCategoriesFilter
    {
        public string Display { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}


