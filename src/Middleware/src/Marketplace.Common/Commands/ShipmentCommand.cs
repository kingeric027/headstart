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
using SmartyStreets.USAutocompleteApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Misc = Marketplace.Common.Models.Misc;

namespace Marketplace.Common.Commands
{
    public class DocumentRowError
    {
        public int Row { get; set; }
        public string ErrorMessage { get; set; }
        public int Column { get; set; }
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
        public int ProcessFailureListCount { get; set; }
        public int DocumentFailureListCount { get; set; }

    }

    public class BatchProcessFailure
    {
        public Misc.Shipment Shipment { get; set; }
        public string Error { get; set; }
    }

    public class BatchProcessResult
    {
        public BatchProcessSummary Meta { get; set; }
        public List<Shipment> SuccessfulList = new List<Shipment>();
        public List<BatchProcessFailure> ProcessFailureList = new List<BatchProcessFailure>();
        public List<DocumentRowError> InvalidRowFailureList = new List<DocumentRowError>();


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
        private Dictionary<string, Shipment> _shipmentByTrackingNumber = new Dictionary<string, Shipment>();

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

            if (file == null) { return new DocumentImportResult(); }
            using Stream stream = file.OpenReadStream();
            List<RowInfo<Misc.Shipment>> shipments = new Mapper(stream).Take<Misc.Shipment>(0, 1000).ToList();

            DocumentImportResult result = Validate(shipments);

            processResults = await ProcessShipments(result, accessToken);

            return await Task.FromResult(result);
        }

        private async Task<BatchProcessResult> ProcessShipments(DocumentImportResult importResult, string accessToken)
        {
            BatchProcessResult processResult = new BatchProcessResult();

            if (importResult == null) { return null; }
            if (importResult.Valid?.Count < 0) { return null; }

            foreach (Misc.Shipment shipment in importResult.Valid)
            {
                try
                {
                    bool isSuccessful = await ProcessShipment(shipment, processResult, accessToken);
                }
                catch (Exception ex)
                {
                    BatchProcessFailure failureDto = new BatchProcessFailure();
                    failureDto.Error = ex.Message;
                    failureDto.Shipment = shipment;
                    processResult.ProcessFailureList.Add(failureDto);
                }

            }
            processResult.InvalidRowFailureList.AddRange(importResult.Invalid);
            processResult.Meta = new BatchProcessSummary()
            {
                ProcessFailureListCount = processResult.ProcessFailureList.Count(),
                DocumentFailureListCount = processResult.InvalidRowFailureList.Count(),
                SuccessfulCount = processResult.SuccessfulList.Count(),
                TotalCount = importResult.Meta.TotalCount
                
            };

            return processResult;

        }

        private BatchProcessFailure CreateBatchProcessFailureItem(Misc.Shipment shipment, string errorMessage)
        {
            BatchProcessFailure failure = new BatchProcessFailure();

            if (errorMessage == null) { failure.Error = "Something went wrong"; }
            else { failure.Error = errorMessage; }

            failure.Shipment = shipment;

            return failure;
        }

        private async Task<bool> ProcessShipment(Misc.Shipment shipment, BatchProcessResult result, string accessToken)
        {
            PartialShipment newShipment = null;
            Shipment ocShipment;
            try
            {
                Order ocOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, shipment.OrderID);
                LineItem lineItem = await _oc.LineItems.GetAsync(OrderDirection.Incoming, shipment.OrderID, shipment.LineItemID);
                ShipmentItem newShipmentItem = new ShipmentItem()
                {
                    OrderID = shipment.OrderID,
                    LineItemID = lineItem.ID,
                    QuantityShipped = Convert.ToInt32(shipment.QuantityShipped),
                    UnitPrice = Convert.ToDecimal(shipment.Cost)
                };

                ocShipment = await GetShipmentByTrackingNumber(shipment, accessToken);
                //If a user included a ShipmentID in the spreadsheet, find that shipment and patch it with the information on that row
                
                if (ocShipment != null)
                {
                    newShipment = PatchShipment(ocShipment, shipment);
                }

                if (newShipment != null)
                {
                    Shipment processedShipment = await _oc.Shipments.PatchAsync(newShipment.ID, newShipment, accessToken);
                    //POST a shipment item, passing it a Shipment ID parameter, and a request body of Order ID, Line Item ID, and Quantity Shipped
                    await _oc.Shipments.SaveItemAsync(shipment.ShipmentID, newShipmentItem);

                    //Re-patch the shipment adding the date shipped now due to oc bug
                    var repatchedShipment = PatchShipment(ocShipment, shipment);
                    await _oc.Shipments.PatchAsync(newShipment.ID, repatchedShipment);


                    result.SuccessfulList.Add(processedShipment);
                }
                if (lineItem.ID == null)
                {
                    //Create new lineItem
                    await _oc.Shipments.SaveItemAsync(shipment.ShipmentID, newShipmentItem);
                }
                return true;
            }
            catch (OrderCloudException ex)
            {
                result.ProcessFailureList.Add(CreateBatchProcessFailureItem(shipment, $"{ex.Message}: {ex.Data.Keys}"));
                return false;
            }
        }

        private async Task<Shipment> GetShipmentByTrackingNumber(Misc.Shipment shipment, string accessToken)
        {
            Shipment shipmentResponse;

            //if dictionary already contains shipment, return that shipment
            if (shipment != null && _shipmentByTrackingNumber.ContainsKey(shipment.TrackingNumber)) 
            { 
                return _shipmentByTrackingNumber[shipment.TrackingNumber]; 
            }

            if (shipment?.ShipmentID != null)
            {
                //get shipment if shipmentId is provided
                shipmentResponse = await _oc.Shipments.GetAsync(shipment.ShipmentID, accessToken);
                if (shipmentResponse != null)
                {
                    //add shipment to dictionary if it's found
                    _shipmentByTrackingNumber.Add(shipmentResponse.TrackingNumber, shipmentResponse);
                }
            } 
            else if (shipment?.ShipmentID == null)
            {
                PartialShipment newShipment = PatchShipment(null, shipment);
                //Create shipment for tracking number provided if shipmentId wasn't included
                Shipment createdShipment = await _oc.Shipments.CreateAsync(newShipment, accessToken);
                if (createdShipment != null)
                {
                    _shipmentByTrackingNumber.Add(createdShipment.TrackingNumber, createdShipment);
                }
            }
            return null;
        }
        

        private PartialShipment PatchShipment(Shipment ocShipment, Misc.Shipment shipment)
        {
            PartialShipment newShipment = new PartialShipment();
            bool isCreatingNew = false;

            if (ocShipment == null)
            {
                isCreatingNew = true;
            } 
            else
            {
                newShipment.ID = ocShipment?.ID;
                newShipment.xp = ocShipment?.xp;
                newShipment.FromAddress = ocShipment?.FromAddress;
                newShipment.ToAddress = ocShipment?.ToAddress;
            }
            if (newShipment.xp == null)
            {
                newShipment.xp = new ShipmentXp();
            }
            newShipment.BuyerID = shipment.BuyerID;
            newShipment.Shipper = shipment.Shipper;
            newShipment.DateShipped = isCreatingNew? null : shipment.DateShipped; //Must patch to null on new creation due to OC bug
            newShipment.DateDelivered = shipment.DateDelivered;
            newShipment.TrackingNumber = shipment.TrackingNumber;
            newShipment.Cost = Convert.ToDecimal(shipment.Cost);
            newShipment.Account = shipment.Account;        
            newShipment.FromAddressID = shipment.FromAddressID;
            newShipment.ToAddressID = shipment.ToAddressID;
            newShipment.xp.Service = Convert.ToString(shipment.Service);
            newShipment.xp.Comment = Convert.ToString(shipment.Comment);

            return newShipment;
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
                        Row = row.RowNumber++,
                        Column = row.ErrorColumnIndex
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
