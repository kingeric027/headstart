using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration.Models
{
    public class ShipmentSyncOrder
    {
        public string OrderCloudOrderID { get; set; }
        public string FreightPopOrderID { get; set; }
        public MarketplaceOrder Order { get; set; }
        public Response<List<ShipmentDetails>> FreightPopShipmentResponses { get; set; }
    }
}
