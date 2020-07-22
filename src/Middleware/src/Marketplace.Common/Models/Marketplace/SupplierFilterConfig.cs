using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using ordercloud.integrations.library;
using ordercloud.integrations.cms;
using Newtonsoft.Json.Linq;

namespace Marketplace.Models
{
    [SwaggerModel]
    public class SupplierFilterConfigDocument : Document<SupplierFilterConfig>
    {
    }

    [SwaggerModel]
    public class SupplierFilterConfig
    {
        public string Display { get; set; }
        public string Path { get; set; }
        public List<Filter> Items { get; set; }
        public bool AllowSupplierEdit { get; set; }
        public bool AllowSellerEdit { get; set; }
        public BuyerAppFilterType BuyerAppFilterType { get; set; }
    }

    [SwaggerModel]
    public class Filter
    {
        public string Text {get; set;}
        public string Value {get; set; }
    }

    public enum BuyerAppFilterType
    {
       SelectOption,
       NonUI
    }

}


