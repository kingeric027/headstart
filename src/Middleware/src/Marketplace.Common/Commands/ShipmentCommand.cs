using Dynamitey;
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

    public class BatchProcessSummary
    {
        public int TotalCount { get; set; }
        public int SuccessfulCount { get; set; }
        public int FailureListCount { get; set; }
    }

    public class BatchProcessResult
    {
        public BatchProcessSummary Meta { get; set; }
        public List<Misc.Shipment> SuccessfulList = new List<Misc.Shipment>();
        public List<Misc.Shipment> FailureList = new List<Misc.Shipment>();

    }

    public interface IShipmentCommand
    {
        Task<SuperShipment> CreateShipment(SuperShipment superShipment, string supplierToken);
        Task<DocumentImportResult> UploadShipments(IFormFile file, string supplierToken);
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
            ShipmentItem firstShipmentItem = superShipment.ShipmentItems.First();
            string supplierOrderID = firstShipmentItem.OrderID;
            string buyerOrderID = supplierOrderID.Split("-").First();

            // in the platform, in order to make sure the order has the proper Order.Status, you must 
            // create a shipment without a DateShipped and then patch the DateShipped after
            DateTimeOffset? dateShipped = superShipment.Shipment.DateShipped;
            superShipment.Shipment.DateShipped = null;


            await PatchLineItemStatuses(supplierOrderID, superShipment);
            string buyerID = await GetBuyerIDForSupplierOrder(firstShipmentItem.OrderID);
            superShipment.Shipment.BuyerID = buyerID;

            MarketplaceShipment ocShipment = await _oc.Shipments.CreateAsync<MarketplaceShipment>(superShipment.Shipment, accessToken: supplierToken);
            IList<ShipmentItem> shipmentItemResponses = await Throttler.RunAsync(
                superShipment.ShipmentItems,
                100,
                5,
                (shipmentItem) => _oc.Shipments.SaveItemAsync(ocShipment.ID, shipmentItem, accessToken: supplierToken));
            MarketplaceShipment ocShipmentWithDateShipped = await _oc.Shipments.PatchAsync<MarketplaceShipment>(ocShipment.ID, new PartialShipment() { DateShipped = dateShipped }, accessToken: supplierToken);
            return new SuperShipment()
            {
                Shipment = ocShipmentWithDateShipped,
                ShipmentItems = shipmentItemResponses.ToList()
            };
        }

        private async Task PatchLineItemStatuses(string supplierOrderID, SuperShipment superShipment)
        {
            List<LineItemStatusChange> lineItemStatusChanges = superShipment.ShipmentItems.Select(shipmentItem =>
            {
                return new LineItemStatusChange()
                {
                    Quantity = shipmentItem.QuantityShipped,
                    ID = shipmentItem.LineItemID
                };
            }).ToList();

            LineItemStatusChanges lineItemStatusChange = new LineItemStatusChanges()
            {
                Changes = lineItemStatusChanges,
                Status = LineItemStatus.Complete
            };

            await _lineItemCommand.UpdateLineItemStatusesAndNotifyIfApplicable(OrderDirection.Outgoing, supplierOrderID, lineItemStatusChange);
        }

        private async Task<string> GetBuyerIDForSupplierOrder(string supplierOrderID)
        {
            string buyerOrderID = supplierOrderID.Split("-").First();
            Order relatedBuyerOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, buyerOrderID);
            return relatedBuyerOrder.FromCompanyID;
        }

        public async Task<DocumentImportResult> UploadShipments(IFormFile file, string accessToken)
        {
            DocumentImportResult documentImportResult;


            documentImportResult = await GetShipmentListFromFile(file, accessToken);

            return documentImportResult;
        }

        private async Task<DocumentImportResult> GetShipmentListFromFile(IFormFile file, string accessToken)
        {
            BatchProcessResult processResults;
            using Stream stream = file.OpenReadStream();
            List<RowInfo<Misc.Shipment>> shipments = new Mapper(stream).Take<Misc.Shipment>(0, 1000).ToList();

            DocumentImportResult result = Validate(shipments.Where(p => p.Value?.LineItemID != null).Select(p => p).ToList());

            processResults = ProcessShipments(result, accessToken);

            return await Task.FromResult(result);
        }

        private BatchProcessResult ProcessShipments(DocumentImportResult importResult, string accessToken)
        {
            BatchProcessResult processResult = new BatchProcessResult();

            if (importResult == null) { return null; }
            if (importResult.Valid?.Count < 0) { return null; }

            foreach (Misc.Shipment shipment in importResult.Valid)
            {
                bool isSuccessful = ProcessShipment(shipment, accessToken);
                if (isSuccessful) { processResult.SuccessfulList.Add(shipment); }
                else { processResult.FailureList.Add(shipment); }
            }

            processResult.Meta = new BatchProcessSummary()
            {
                FailureListCount = processResult.FailureList.Count(),
                SuccessfulCount = processResult.SuccessfulList.Count(),
                TotalCount = importResult.Meta.TotalCount
            };

            return processResult;

        }

        private bool ProcessShipment(Misc.Shipment shipment, string accessToken)
        {
            var order = _oc.Shipments.GetAsync(shipment.ShipmentID, accessToken);
            return false;
        }

        public static DocumentImportResult Validate(List<RowInfo<Misc.Shipment>> rows)
        {
            DocumentImportResult result = new DocumentImportResult()
            {
                Invalid = new List<DocumentRowError>(),
                Valid = new List<Misc.Shipment>()
            };

            foreach (RowInfo<Misc.Shipment> row in rows)
            {
                if (row.ErrorColumnIndex > -1)
                    result.Invalid.Add(new DocumentRowError()
                    {
                        ErrorMessage = row.ErrorMessage,
                        Row = row.RowNumber++
                    });
                else
                {
                    List<ValidationResult> results = new List<ValidationResult>();
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
