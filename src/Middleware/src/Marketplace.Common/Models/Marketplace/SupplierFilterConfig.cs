using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using ordercloud.integrations.library;
using ordercloud.integrations.cms;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

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


        // Either SelectOption or NonUi
        // we can't use an enum because it comes through as int and json validator expects string
        public string BuyerAppFilterType { get; set; }
    }

    [SwaggerModel]
    public class Filter
    {
        public string Text {get; set;}
        public string Value {get; set; }
    }

    public class BuyerAppFilterType
    {
        public const string SelectOption = "SelectOption";
        public const string NonUI = "NonUI";
    }

}


