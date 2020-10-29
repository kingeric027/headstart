using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Extended;
using Marketplace.Models.Models.Marketplace;
using Microsoft.AspNetCore.Http;
using Npoi.Mapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Misc = Marketplace.Common.Models.Misc;

namespace Marketplace.Common.Commands
{
    public class DocumentRowError
    {
        public int Row { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class DocumentImportSummary
    {
        public int TotalCount { get; set; }
        public int ValidCount { get; set; }
        public int InvalidCount { get; set; }
    }

    public class DocumentImportResult
    {
        public DocumentImportSummary Meta { get; set; }
        public List<Misc.Shipment> Valid = new List<Misc.Shipment>();
        public List<DocumentRowError> Invalid = new List<DocumentRowError>();
    }

    public interface IShipmentCommand
    {
        Task<SuperShipment> CreateShipment(SuperShipment superShipment, string supplierToken);
        Task<DocumentImportResult> UploadShipments(IFormFile file);
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

        public async Task<DocumentImportResult> UploadShipments(IFormFile file)
        {
            DocumentImportResult documentImportResult;

            documentImportResult = await GetShipmentListFromFile(file);

            return documentImportResult;
        }

        private async Task<DocumentImportResult> GetShipmentListFromFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var shipments = new Mapper(stream).Take<Misc.Shipment>(0, 1000).ToList();
            var result = Validate(shipments.Where(p => p.Value?.LineItemID != null).Select(p => p).ToList());
            return await Task.FromResult(result);
        }

        public static DocumentImportResult Validate(List<RowInfo<Misc.Shipment>> rows)
        {
            var result = new DocumentImportResult()
            {
                Invalid = new List<DocumentRowError>(),
                Valid = new List<Misc.Shipment>()
            };

            foreach (var row in rows)
            {
                if (row.ErrorColumnIndex > -1)
                    result.Invalid.Add(new DocumentRowError()
                    {
                        ErrorMessage = row.ErrorMessage,
                        Row = row.RowNumber++
                    });
                else
                {
                    var results = new List<ValidationResult>();
                    if (Validator.TryValidateObject(row.Value, new ValidationContext(row.Value), results, true) == false)
                    {
                        result.Invalid.Add(new DocumentRowError()
                        {
                            ErrorMessage = $"{results.FirstOrDefault()?.ErrorMessage}",
                            Row = row.RowNumber++
                        });
                    }
                    else
                    {
                        result.Valid.Add(row.Value);
                    }
                }
            }

            result.Meta = new DocumentImportSummary()
            {
                InvalidCount = result.Invalid.Count,
                ValidCount = result.Valid.Count,
                TotalCount = rows.Count
            };
            return result;
        }
    }
}
