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
        public List<MarketplaceOrder> OrdersToSync { get; set; }
        public List<ShipmentSyncOrder> ShipmentSyncOrders { get; set; }
    }
    [DocIgnore]
    public class OrderWorkItem
    {
        public MarketplaceSupplier Supplier { get; set; }
        public string SupplierToken { get; set; }
        public ShipmentSyncOrder ShipmentSyncOrder { get; set; }
    }
    [DocIgnore]
    public class ShipmentSyncOrder
    {
        public string OrderCloudOrderID { get; set; }
        public string FreightPopOrderID { get; set; }
        public MarketplaceOrder Order { get; set; }
        public Response<List<ShipmentDetails>> FreightPopShipmentResponses { get; set; }
    }
}
