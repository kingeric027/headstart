using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderCloud.SDK;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Common.Services.ShippingIntegration;
using ordercloud.integrations.avalara;
using ordercloud.integrations.library;
using ordercloud.integrations.exchangerates;
using Newtonsoft.Json.Converters;
using Marketplace.Models.Extended;
using Microsoft.WindowsAzure.Storage;

namespace Marketplace.Common.Commands
{
    public interface IPostSubmitCommand
    {
        Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet order);
    }

    public class PostSubmitCommand : IPostSubmitCommand
    {
        private readonly IOrderCloudClient _oc;

        // temporary service until we get updated sdk
        private readonly IZohoCommand _zoho;
        private readonly IAvalaraCommand _avalara;
        private readonly IExchangeRatesCommand _exchangeRates;
        private readonly ISendgridService _sendgridService;
        private readonly ILocationPermissionCommand _locationPermissionCommand;
        private readonly IOrderCommand _orderCommand;
        private readonly ILineItemCommand _lineItemCommand;

        public PostSubmitCommand(IExchangeRatesCommand exchangeRates, ILocationPermissionCommand locationPermissionCommand, ISendgridService sendgridService, IAvalaraCommand avatax, IOrderCloudClient oc, IZohoCommand zoho, IOrderCommand orderCommand, ILineItemCommand lineItemCommand)
        {
            _oc = oc;
            _avalara = avatax;
            _zoho = zoho;
            _sendgridService = sendgridService;
            _locationPermissionCommand = locationPermissionCommand;
            _exchangeRates = exchangeRates;
            _orderCommand = orderCommand;
            _lineItemCommand = lineItemCommand;
        }

        private class PostSubmitErrorResponse
        {
            public List<ProcessResult> ProcessResults { get; set; }
        }

        private class ProcessResult
        {
            //public bool Failed { get; set; }
            public ProcessType Type { get; set; }
            //public Exception ErrorDetail { get; set; }
            public List<ProcessResultAction> Activity { get; set; } = new List<ProcessResultAction>();
        }

        private class ProcessResultAction
        {
            public ProcessType ProcessType { get; set; }
            public string Description { get; set; }
            public bool Success { get; set; }
            public ProcessResultException Exception { get; set; }
        }

        private class ProcessResultException
        {
            public ProcessResultException(Exception ex)
            {
                this.Message = ex.Message;
            }

            public string Message { get; set; }
        }

        [JsonConverter(typeof(StringEnumConverter))]

        private enum ProcessType
        {
            Forwarding,
            Notification,
            Accounting,
            Tax
        }

        public async Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet orderWorksheet)
        {

            /* post order submit can be broken out into two steps
             * 
             * 1) orders forwarding process
             * 2) integration orchestration
             * 
             * if step 1 fails, step two should not take place; if step 1 succeeds every integration should have a chance to run
             * regardless of the success or failure of other integration processes
             * 
             * step 2 has 4 parts
             * a) sendgrid emailing
             * c) avalara transaction creation
             * d) zoho sales and purchase order creation
             *
             * b, c, and d only run on standard (non-quote) orders
             */

            // maintaining a list of order for marking orders as needing attention
            // if forwarding process fails we are only relying on marking the buyer order

            var results = new List<ProcessResult>();

            // STEP 1
            var (supplierOrders, buyerOrder, activities) = await HandlingForwarding(orderWorksheet);
            results.Add(new ProcessResult()
            {
                Type = ProcessType.Forwarding,
                Activity = activities
            });
            // step 1 failed. we don't want to attempt the integrations. return error for further action
            if (activities.Any(a => !a.Success))
                return await CreateOrderSubmitResponse(results, new List<MarketplaceOrder> { orderWorksheet.Order });
            
            // STEP 2 (integrations)
            var integrations = await HandleIntegrations(supplierOrders, buyerOrder);
            results.AddRange(integrations);

            // STEP 3: return OrderSubmitResponse
            return await CreateOrderSubmitResponse(results, new List<MarketplaceOrder> { orderWorksheet.Order });
        }

        private async Task<List<ProcessResult>> HandleIntegrations(List<MarketplaceOrder> updatedSupplierOrders, MarketplaceOrderWorksheet updatedMarketplaceOrderWorksheet)
        {
            // STEP 1: SendGrid notifications
            var results = new List<ProcessResult>();
            var notifications = await ProcessActivityCall(
                ProcessType.Notification,
                "Sending Order Submit Emails",
                _sendgridService.SendOrderSubmitEmail(updatedMarketplaceOrderWorksheet));
            results.Add(new ProcessResult()
            {
                Type = ProcessType.Notification,
                Activity = new List<ProcessResultAction>() { notifications }
            });

            var tax = await ProcessActivityCall(
                ProcessType.Tax,
                "Creating Tax Transaction",
                HandleTaxTransactionCreationAsync(updatedMarketplaceOrderWorksheet.Reserialize<OrderWorksheet>()));
            results.Add(new ProcessResult()
            {
                Type = ProcessType.Tax,
                Activity = new List<ProcessResultAction>() { tax }
            });

            var erp = await ProcessActivityCall(
                ProcessType.Accounting,
                "Create Order in ERP",
                HandleZohoIntegration(updatedSupplierOrders, updatedMarketplaceOrderWorksheet));
            results.Add(new ProcessResult()
            {
                Type = ProcessType.Accounting,
                Activity = new List<ProcessResultAction>() { erp }
            });

            return results;
        }

        //private async Task<OrderSubmitResponse> HandleIntegrationss(List<MarketplaceOrder> updatedSupplierOrders, MarketplaceOrderWorksheet updatedMarketplaceOrderWorksheet)
        //{
        //    var ordersRelatingToProcess = new List<MarketplaceOrder> { updatedMarketplaceOrderWorksheet.Order };
        //    ordersRelatingToProcess.AddRange(updatedSupplierOrders);

        //    var integrationRequests = new List<Task<ProcessResult>>
        //    {
        //        SafeIntegrationCall(ProcessType.Notification, async () => await _sendgridService.SendOrderSubmitEmail(updatedMarketplaceOrderWorksheet))
        //    };

        //    // quote orders do not need to flow into our integrations
        //    if (IsStandardOrder(updatedMarketplaceOrderWorksheet))
        //    {
        //        integrationRequests.Add(SafeIntegrationCall(ProcessType.Tax, async () => await HandleTaxTransactionCreationAsync(updatedMarketplaceOrderWorksheet.Reserialize<OrderWorksheet>())));
        //        integrationRequests.Add(SafeIntegrationCall(ProcessType.Accounting, async () => await HandleZohoIntegration(updatedSupplierOrders, updatedMarketplaceOrderWorksheet)));
        //    }

        //    var integrationResponses = await Throttler.RunAsync(integrationRequests, 100, 4, (request) => request);

        //    return await CreateOrderSubmitResponse(integrationResponses.ToList(), ordersRelatingToProcess);
        //}

        // don't rely on this
        
        private async Task<OrderSubmitResponse> CreateOrderSubmitResponse(List<ProcessResult> processResults,
            List<MarketplaceOrder> ordersRelatingToProcess)
        {
            try
            {
                if (processResults.All(i => i.Activity.All(a => !a.Success)))
                    return new OrderSubmitResponse()
                    {
                        HttpStatusCode = 200,
                        xp = processResults
                    };
                await MarkOrdersAsNeedingAttention(ordersRelatingToProcess); 
                return new OrderSubmitResponse()
                {
                    HttpStatusCode = 500,
                    xp = new PostSubmitErrorResponse()
                    {
                        ProcessResults = processResults
                    }
                };
            }
            catch (OrderCloudException ex)
            {
                return new OrderSubmitResponse()
                {
                    HttpStatusCode = 500,
                    UnhandledErrorBody = JsonConvert.SerializeObject(ex.Errors)
                };
            }
        }
        
        private async Task MarkOrdersAsNeedingAttention(List<MarketplaceOrder> orders)
        {
            var partialOrder = new PartialOrder() { xp = new { NeedsAttention = true } };

            var orderInfos = new List<Tuple<OrderDirection, string>> { };

            var buyerOrder = orders.First();
            orderInfos.Add(new Tuple<OrderDirection, string>(OrderDirection.Incoming, buyerOrder.ID));
            orders.RemoveAt(0);
            orderInfos.AddRange(orders.Select(o => new Tuple<OrderDirection, string>(OrderDirection.Outgoing, o.ID)));

            await Throttler.RunAsync(orderInfos, 100, 3, (orderInfo) => _oc.Orders.PatchAsync(orderInfo.Item1, orderInfo.Item2, partialOrder));

        }

        private bool IsStandardOrder(MarketplaceOrderWorksheet marketplaceOrderWorkSheet)
        {
            return marketplaceOrderWorkSheet.Order.xp == null || marketplaceOrderWorkSheet.Order.xp.OrderType != OrderType.Quote;
        }

        private async Task HandleZohoIntegration(List<MarketplaceOrder> updatedSupplierOrders, MarketplaceOrderWorksheet updatedMarketplaceOrderWorksheet)
        {
            var zoho_salesorder = await _zoho.CreateSalesOrder(updatedMarketplaceOrderWorksheet);
            //TODO: put into a throttler the lines below
            await _zoho.CreateOrUpdatePurchaseOrder(zoho_salesorder, updatedSupplierOrders);
            await _zoho.CreateShippingPurchaseOrder(zoho_salesorder, updatedMarketplaceOrderWorksheet);
        }

        private static async Task<ProcessResultAction> ProcessActivityCall(ProcessType type, string description, Task func)
        {
            try
            {
                await func;
                return new ProcessResultAction() {
                        ProcessType = type,
                        Description = description,
                        Success = true
                };
            }
            catch (Exception ex)
            {
                return new ProcessResultAction() {
                    Description = description,
                    ProcessType = type,
                    Success = false,
                    Exception = new ProcessResultException(ex)
                };
            }
        }

        private static async Task<Tuple<ProcessResultAction, T>> ProcessActivityCall<T>(ProcessType type, string description, Task<T> func) where T : class, new()
        {
            // T must be a class and be newable so the error response can be handled.
            try
            {
                return new Tuple<ProcessResultAction, T>(
                    new ProcessResultAction()
                    {
                        ProcessType = type,
                        Description = description,
                        Success = true
                    },
                    await func
                );
            }
            catch (Exception ex)
            {
                return new Tuple<ProcessResultAction, T>(new ProcessResultAction()
                {
                    Description = description,
                    ProcessType = type,
                    Success = false,
                    Exception = new ProcessResultException(ex)
                }, new T());
            }
        }

        //private async Task<ProcessResult> SafeIntegrationCall(ProcessType processType, Func<Task> func)
        //{
        //    try
        //    {
        //        await func();
        //        return new ProcessResult()
        //        {
        //            Failed = false,
        //            Type = processType,
        //        };
        //    } catch (Exception ex)
        //    {
        //        return new ProcessResult()
        //        {
        //            Failed = true,
        //            Type = processType,
        //            ErrorDetail = ex
        //        };
        //    }
        //}

        private async Task<Tuple<List<MarketplaceOrder>, MarketplaceOrderWorksheet, List<ProcessResultAction>>> HandlingForwarding(MarketplaceOrderWorksheet orderWorksheet)
        {
            var activities = new List<ProcessResultAction>();
            // forwarding
            var orderForwardAction = ProcessActivityCall(
                ProcessType.Forwarding,
                "OrderCloud API Order.ForwardAsync",
                _oc.Orders.ForwardAsync(OrderDirection.Incoming, orderWorksheet.Order.ID)
            );
            activities.Add(orderForwardAction.Result.Item1);

            var supplierOrders = orderForwardAction.Result.Item2.OutgoingOrders.ToList();

            // creating relationship between the buyer order and the supplier order
            // no relationship exists currently in the platform
            var updatedSupplierOrders = ProcessActivityCall(
                ProcessType.Forwarding, "Create Order Relationships And Transfer XP",
                CreateOrderRelationshipsAndTransferXP(orderWorksheet.Order, supplierOrders));
            activities.Add(updatedSupplierOrders.Result.Item1);

            // need to get fresh order worksheet because this process has changed things about the worksheet
            var updatedWorksheet = ProcessActivityCall(
                ProcessType.Forwarding, 
                "Get Updated Order Worksheet",
                _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderWorksheet.Order.ID));
            activities.Add(updatedWorksheet.Result.Item1);

            return await Task.FromResult(new Tuple<List<MarketplaceOrder>, MarketplaceOrderWorksheet, List<ProcessResultAction>>(updatedSupplierOrders.Result.Item2, updatedWorksheet.Result.Item2, activities));
        }

        private async Task<List<MarketplaceOrder>> CreateOrderRelationshipsAndTransferXP(MarketplaceOrder buyerOrder, List<Order> supplierOrders)
        {
            var updatedSupplierOrders = new List<MarketplaceOrder>();
            var supplierIDs = new List<string>();
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, buyerOrder.ID);
            var shipFromAddressIDs = GetShipFromAddressIDs(lineItems.Items.ToList());

            foreach (var supplierOrder in supplierOrders)
            {
                var supplierID = supplierOrder.ToCompanyID;
                supplierIDs.Add(supplierID);
                var shipFromAddressIDsForSupplierOrder = shipFromAddressIDs.Where(addressID => addressID.Contains(supplierID)).ToList();
                var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierID);
                var supplierOrderPatch = new PartialOrder()
                {
                    ID = $"{buyerOrder.ID}-{supplierID}",
                    xp = GetNewOrderXP(buyerOrder, supplier, shipFromAddressIDsForSupplierOrder)
                };
                var updatedSupplierOrder = await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Outgoing, supplierOrder.ID, supplierOrderPatch);
                updatedSupplierOrders.Add(updatedSupplierOrder);
            }

            await _lineItemCommand.SetInitialSubmittedLineItemStatuses(buyerOrder.ID);

            var buyerOrderPatch = new PartialOrder()
            {
                xp = new
                {
                    ShipFromAddressIDs = shipFromAddressIDs,
                    SupplierIDs = supplierIDs,
                    ClaimStatus = ClaimStatus.NoClaim,
                    ShippingStatus = ShippingStatus.Processing,
                    SubmittedOrderStatus = SubmittedOrderStatus.Open
                }
            };

            await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrder.ID, buyerOrderPatch);
            return updatedSupplierOrders;
        }

        private OrderXp GetNewOrderXP(MarketplaceOrder buyerOrder, MarketplaceSupplier supplier, List<string> shipFromAddressIDsForSupplierOrder)
        {
            var supplierOrderXp = new OrderXp()
            {
                ShipFromAddressIDs = shipFromAddressIDsForSupplierOrder,
                SupplierIDs = new List<string>() { supplier.ID },
                StopShipSync = false,
                OrderType = buyerOrder.xp.OrderType,
                QuoteOrderInfo = buyerOrder.xp.QuoteOrderInfo,
                Currency = supplier.xp.Currency,
                ClaimStatus = ClaimStatus.NoClaim,
                ShippingStatus = ShippingStatus.Processing,
                SubmittedOrderStatus = SubmittedOrderStatus.Open
            };
            return supplierOrderXp;
        }

        private List<string> GetShipFromAddressIDs(List<LineItem> lineItems)
        {
            var shipFromAddressIDs = new List<string>();
            lineItems.ForEach(li => {
                if (!shipFromAddressIDs.Contains(li.ShipFromAddressID))
                {
                    shipFromAddressIDs.Add(li.ShipFromAddressID);
                }
            });
            return shipFromAddressIDs;
        }

        private async Task HandleTaxTransactionCreationAsync(OrderWorksheet orderWorksheet)
        {
            var transaction = await _avalara.CreateTransactionAsync(orderWorksheet);
            await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderWorksheet.Order.ID, new PartialOrder()
            {
                TaxCost = transaction.totalTax ?? 0,  // Set this again just to make sure we have the most up to date info
                xp = new { AvalaraTaxTransactionCode = transaction.code }
            });
        }
    };
}