using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;
using System.Linq;
using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.library;
using System;
using Marketplace.Models.Extended;
using System.Collections.Generic;
using Marketplace.Models.Models.Marketplace;

namespace Marketplace.Common.Commands
{
    public interface IShipmentCommand
    {
        Task<ShipmentCreateResponse> CreateShipment(SuperShipment superShipment, string supplierToken);
    }
    public class ShipmentCommand : IShipmentCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly ILineItemCommand _lineItemCommand;

        public ShipmentCommand(AppSettings settings, IOrderCloudClient oc, ILineItemCommand lineItemCommand)
        {
            _oc = oc;
            _lineItemCommand = lineItemCommand;
        }
        public async Task<ShipmentCreateResponse> CreateShipment(SuperShipment superShipment, string supplierToken)
        {
            var firstShipmentItem = superShipment.ShipmentItems.First();
            var supplierOrderID = firstShipmentItem.OrderID;
            var buyerOrderID = supplierOrderID.Split("-").First();
            
            // in the platform, in order to make sure the order has the proper Order.Status, you must 
            // create a shipment without a DateShipped and then patch the DateShipped after
            var dateShipped = superShipment.Shipment.DateShipped;
            superShipment.Shipment.DateShipped = null;


            await PatchLineItemStatuses(supplierOrderID, superShipment);
            var buyerID = await GetBuyerIDForSupplierOrder(firstShipmentItem.OrderID);
            superShipment.Shipment.BuyerID = buyerID;

            var ocShipment = await _oc.Shipments.CreateAsync<MarketplaceShipment>(superShipment.Shipment, accessToken: supplierToken);
            var shipmentItemResponses = await Throttler.RunAsync(
                superShipment.ShipmentItems, 
                100, 
                5, 
                (shipmentItem) => _oc.Shipments.SaveItemAsync(ocShipment.ID, shipmentItem, accessToken: supplierToken));
            var ocShipmentWithDateShipped = await _oc.Shipments.PatchAsync<MarketplaceShipment>(ocShipment.ID, new PartialShipment() { DateShipped = dateShipped }, accessToken: supplierToken);
            return new ShipmentCreateResponse()
            {
                Shipment = ocShipmentWithDateShipped,
                ShipmentItems = shipmentItemResponses.ToList()
            };
        }

        private async Task PatchLineItemStatuses(string supplierOrderID, SuperShipment superShipment)
        {
            var lineItemStatusChanges = superShipment.ShipmentItems.Select(shipmentItem =>
            {
                return new LineItemStatusChange()
                {
                    Quantity = shipmentItem.QuantityShipped,
                    ID = shipmentItem.LineItemID
                };
            }).ToList();

            var lineItemStatusChange = new LineItemStatusChanges()
            {
                Changes = lineItemStatusChanges,
                Status = LineItemStatus.Complete
            };

            await _lineItemCommand.UpdateLineItemStatusesAndNotifyIfApplicable(OrderDirection.Outgoing, supplierOrderID, lineItemStatusChange);
        }

        private async Task<string> GetBuyerIDForSupplierOrder(string supplierOrderID)
        {
            var buyerOrderID = supplierOrderID.Split("-").First();
            var relatedBuyerOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, buyerOrderID);
            return relatedBuyerOrder.FromCompanyID;
        }
    }
}
