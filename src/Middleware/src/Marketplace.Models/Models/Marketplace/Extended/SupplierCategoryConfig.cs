using Marketplace.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
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

    public class SupplierCategoriesFilter
    {
        public string Display { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}


