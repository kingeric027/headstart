using ordercloud.integrations.freightpop;
using Marketplace.Models.Models.Marketplace;
using System.Collections.Generic;
using ordercloud.integrations.library;

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
