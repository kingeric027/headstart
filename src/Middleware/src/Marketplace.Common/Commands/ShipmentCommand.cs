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
using System.Dynamic;

using Microsoft.AspNetCore.Http;
using Misc = Marketplace.Common.Models.Misc;

namespace Marketplace.Common.Commands
{
    public interface IShipmentCommand
    {
        Task<SuperShipment> CreateShipment(SuperShipment superShipment, string supplierToken);
        Task<Misc.UploadShipmentResponse> UploadShipments(IFormFile file);
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
        public async Task<SuperShipment> CreateShipment(SuperShipment superShipment, string supplierToken)
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

            //  platform bug. Cant save new xp values onto shipment line item. Update order line item to have this value
            var shipmentItemsWithComment = superShipment.ShipmentItems.Where(s => s.xp?.Comment != null);
            await Throttler.RunAsync(shipmentItemsWithComment, 100, 5, (shipmentItem) => {
                dynamic comments = new ExpandoObject();
                var commentsByShipment = comments as IDictionary<string, object>;
                commentsByShipment[ocShipment.ID] = shipmentItem.xp?.Comment;

                return _oc.LineItems.PatchAsync(OrderDirection.Incoming, buyerOrderID, shipmentItem.LineItemID,
                    new PartialLineItem()
                    {
                        xp = new
                        {
                            Comments = commentsByShipment
                        }
                    });
            });
            var shipmentItemResponses = await Throttler.RunAsync(
                superShipment.ShipmentItems,
                100,
                5,
                (shipmentItem) => _oc.Shipments.SaveItemAsync(ocShipment.ID, shipmentItem, accessToken: supplierToken));
            var ocShipmentWithDateShipped = await _oc.Shipments.PatchAsync<MarketplaceShipment>(ocShipment.ID, new PartialShipment() { DateShipped = dateShipped }, accessToken: supplierToken);
            return new SuperShipment()
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

        public async Task<Misc.UploadShipmentResponse> UploadShipments(IFormFile file)
        {
            Misc.UploadShipmentResponse response;
            List<Misc.Shipment> shipmentList;

            shipmentList = await GetShipmentListFromFile(file);
            response = new Misc.UploadShipmentResponse();
            response.ErrorList = new List<Misc.Error>();

            response.SuccessfulShipments = shipmentList;

            try
            {

            }
            catch (Exception ex)
            {
                response.ErrorList.Add(new Misc.Error() { ErrorMessage = ex.Message, StackTrace = ex.StackTrace });
                Console.WriteLine(ex);
            }

            return response;
        }

        private Task<List<Misc.Shipment>> GetShipmentListFromFile(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
