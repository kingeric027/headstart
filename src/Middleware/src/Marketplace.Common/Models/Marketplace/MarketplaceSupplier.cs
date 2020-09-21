using System;
using System.Collections.Generic;
using Marketplace.Models.Marketplace.Extended;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models.Models.Marketplace
{
    [SwaggerModel]
    public class MarketplaceSupplier : Supplier<SupplierXp>, IMarketplaceObject
    {
    }

    [SwaggerModel]
    public class SupplierXp
    {
        public string Description { get; set; }
        public Contact SupportContact { get; set; }
        public bool SyncFreightPop { get; set; }
        public string ApiClientID { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public CurrencySymbol? Currency { get; set; } = null;
        public List<ProductType> ProductTypes { get; set; }
        public List<string> CountriesServicing { get; set; }
        public List<SupplierCategory> Categories { get; set; }
        public List<string> NotificationRcpts { get; set; }
        public Nullable<int> FreeShippingThreshold { get; set; }
    }

    [SwaggerModel]
    public class SupplierCategory
    {
        public string ServiceCategory { get; set; }
        public string VendorLevel { get; set; }
    }
}
