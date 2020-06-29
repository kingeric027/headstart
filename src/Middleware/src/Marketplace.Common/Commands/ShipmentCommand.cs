using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.library;
using System;
using Marketplace.Models.Extended;

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
            var ocShipment = await _oc.Shipments.CreateAsync<MarketplaceShipment>(superShipment.Shipment, accessToken: supplierToken);
            var shipmentItemResponses = await Throttler.RunAsync(
                superShipment.ShipmentItems, 
                100, 
                5, 
                (shipmentItem) => _oc.Shipments.SaveItemAsync(ocShipment.ID, shipmentItem, accessToken: supplierToken));
            await PatchShipmentStatus(firstShipmentItem.OrderID);
            return new ShipmentCreateResponse()
            {
                Shipment = ocShipment,
                ShipmentItems = shipmentItemResponses.ToList()
            };
        }

        private async Task PatchShipmentStatus(string supplierOrderID)
        {
            var buyerOrderID = supplierOrderID.Split("-").First();
            var relatedBuyerOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, buyerOrderID);
            var shipments = await _oc.Shipments.ListAsync(buyerOrderID);
            var partiallyShippedOrder = new PartialOrder { xp = new { ShippingStatus = ShippingStatus.PartiallyShipped } };
            var fullyShippedOrder = new PartialOrder { xp = new { ShippingStatus = ShippingStatus.Shipped } };
            if (shipments.Meta.TotalCount == relatedBuyerOrder.LineItemCount)
            {
                await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrderID, fullyShippedOrder);
            } else if (shipments.Meta.TotalCount < relatedBuyerOrder.LineItemCount)
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
