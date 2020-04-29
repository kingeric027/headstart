using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Models.Marketplace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Models.Models.Misc
{
    [DocIgnore]
    public class SupplierShipmentSyncWorkItem
    {
        public MarketplaceSupplier Supplier { get; set; }
        public string SupplierToken { get; set; }
        public List<ShipmentDetails> ShipmentsToSync { get; set; }
    }
    public class ShipmentWorkItem
    {
        public MarketplaceSupplier Supplier { get; set; }
        public string SupplierToken { get; set; }
        public ShipmentDetails Shipment { get; set; }
    }
}
