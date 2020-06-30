using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.library;
using System;
using Marketplace.Models.Extended;
using System.Collections.Generic;

namespace Marketplace.Common.Commands
{
    public interface IShipmentCommand
    {
        Task<ShipmentCreateResponse> CreateShipment(SuperShipment superShipment, string supplierToken);
    }
    public class ShipmentCommand : IShipmentCommand
    {
        private readonly IOrderCloudClient _oc;

        public ShipmentCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _oc = oc;
        }
        public async Task<ShipmentCreateResponse> CreateShipment(SuperShipment superShipment, string supplierToken)
        {
            var firstShipmentItem = superShipment.ShipmentItems.First();
            var buyerID = await GetBuyerIDForSupplierOrder(firstShipmentItem.OrderID);
            superShipment.Shipment.BuyerID = buyerID;
            var buyerOrderID = firstShipmentItem.OrderID.Split("-").First();
            var ocShipment = await _oc.Shipments.CreateAsync<MarketplaceShipment>(superShipment.Shipment, accessToken: supplierToken);
            var shipmentItemResponses = await Throttler.RunAsync(
                superShipment.ShipmentItems, 
                100, 
                5, 
                (shipmentItem) => _oc.Shipments.SaveItemAsync(ocShipment.ID, shipmentItem, accessToken: supplierToken));
            await PatchShipmentStatus(buyerOrderID, superShipment);
            return new ShipmentCreateResponse()
            {
                Shipment = ocShipment,
                ShipmentItems = shipmentItemResponses.ToList()
            };
        }

        private async Task PatchShipmentStatus(string buyerOrderID, SuperShipment superShipment)
        {
            var partiallyShippedOrder = new PartialOrder { xp = new { ShippingStatus = ShippingStatus.PartiallyShipped } };
            var fullyShippedOrder = new PartialOrder { xp = new { ShippingStatus = ShippingStatus.Shipped } };
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, buyerOrderID);
            var qty = 0;
            var qtyShipped = 0;
            foreach (var item in lineItems.Items)
            {
                qty += item.Quantity;
                qtyShipped += item.QuantityShipped;
            }
            if (qty == qtyShipped + superShipment.ShipmentItems.Count) 
            {
                await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrderID, fullyShippedOrder);
            } else if (qtyShipped + superShipment.ShipmentItems.Count < qty)
            {
                await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrderID, partiallyShippedOrder);
            }
        }

        private async Task<string> GetBuyerIDForSupplierOrder(string supplierOrderID)
        {
            var buyerOrderID = supplierOrderID.Split("-").First();
            var relatedBuyerOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, buyerOrderID);
            return relatedBuyerOrder.FromCompanyID;
        }
    }
}
