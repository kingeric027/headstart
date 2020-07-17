using System;
using System.Linq;
using System.Collections.Generic;
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
using ordercloud.integrations.freightpop;
using Newtonsoft.Json.Converters;
using Marketplace.Models.Extended;

namespace Marketplace.Common.Commands
{
    public interface IPostSubmitCommand
    {
        Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet order);
    }

    public class PostSubmitCommand : IPostSubmitCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;

        // temporary service until we get updated sdk
        private readonly IOrderCloudSandboxService _ocSandboxService;
        private readonly IZohoCommand _zoho;
        private readonly IAvalaraCommand _avalara;
		private readonly IExchangeRatesCommand _exchangeRates;
        private readonly ISendgridService _sendgridService;
        private readonly ILocationPermissionCommand _locationPermissionCommand;
        private readonly IOrderCommand _orderCommand;
        
        public PostSubmitCommand(IExchangeRatesCommand exchangeRates, ILocationPermissionCommand locationPermissionCommand, IFreightPopService freightPopService, ISendgridService sendgridService, IAvalaraCommand avatax, IOrderCloudClient oc, IZohoCommand zoho, IOrderCloudSandboxService orderCloudSandboxService, IOrderCommand orderCommand)
        {
            _freightPopService = freightPopService;
			_oc = oc;
            _avalara = avatax;
            _zoho = zoho;
            _sendgridService = sendgridService;
            _ocSandboxService = orderCloudSandboxService;
            _locationPermissionCommand = locationPermissionCommand;
			_exchangeRates = exchangeRates;
            _orderCommand = orderCommand;

		}

        private class PostSubmitErrorResponse
        {
            public List<ProcessResult> ProcessResults { get; set; }
        }
        
        private class ProcessResult
        {
            public bool Failed { get; set; }
            public ProcessType Type { get; set; }
            public Exception ErrorDetail { get; set; }
        }

        [JsonConverter(typeof(StringEnumConverter))]

        private enum ProcessType
        {
            Forwarding,
            Sengrid,
            FreightPop,
            Zoho,
            Avalara
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
             * b) freightpop order importing
             * c) avalara transaction creation
             * d) zoho sales and purchase order creation
             *
             * b, c, and d only run on standard (non-quote) orders
             */

            // maintaining a list of order for marking orders as needing attention
            // if forwarding process fails we are only relying on marking the buyer order
            var ordersRelatingToProcess = new List<MarketplaceOrder> { orderWorksheet.Order };

            try
            {
                var (updatedSupplierOrders, updatedOrderWorksheet) = await HandlingForwarding(orderWorksheet);
                return await HandleIntegrations(updatedSupplierOrders, updatedOrderWorksheet);
            }
            catch (Exception ex)
            {
                var processResults = new List<ProcessResult>
                    {
                        new ProcessResult()
                        {
                            Failed = true,
                            Type = ProcessType.Forwarding,
                            ErrorDetail = ex
                        }
                    };
                return await CreateOrderSubmitResponse(processResults, ordersRelatingToProcess);
            }
        }

        private async Task<OrderSubmitResponse> HandleIntegrations(List<MarketplaceOrder> updatedSupplierOrders, MarketplaceOrderWorksheet updatedMarketplaceOrderWorksheet)
        {
            var ordersRelatingToProcess = new List<MarketplaceOrder> { updatedMarketplaceOrderWorksheet.Order };
            ordersRelatingToProcess.AddRange(updatedSupplierOrders);

            var integrationRequests = new List<Task<ProcessResult>> { };

            integrationRequests.Add(SafeIntegrationCall(ProcessType.Sengrid, async () => await _sendgridService.SendOrderSubmitEmail(updatedMarketplaceOrderWorksheet)));

            // quote orders do not need to flow into our integrations
            if (IsStandardOrder(updatedMarketplaceOrderWorksheet))
            {
                integrationRequests.Add(SafeIntegrationCall(ProcessType.FreightPop, async () => await ImportSupplierOrdersIntoFreightPop(updatedSupplierOrders)));
                integrationRequests.Add(SafeIntegrationCall(ProcessType.Avalara, async () => await HandleTaxTransactionCreationAsync(updatedMarketplaceOrderWorksheet.Reserialize<OrderWorksheet>())));
                integrationRequests.Add(SafeIntegrationCall(ProcessType.Zoho, async () => await HandleZohoIntegration(updatedSupplierOrders, updatedMarketplaceOrderWorksheet)));
            }

            var integrationResponses = await Throttler.RunAsync(integrationRequests, 100, 4, (request) =>
            {
                return request;
            });

            return await CreateOrderSubmitResponse(integrationResponses.ToList(), ordersRelatingToProcess);
        }

        private async Task<OrderSubmitResponse> CreateOrderSubmitResponse(List<ProcessResult> processResults, List<MarketplaceOrder> ordersRelatingToProcess)
        {
            try
            {
                if (processResults.Any(i => i.Failed))
                {
                    await MarkOrdersAsNeedingAttention(ordersRelatingToProcess);

                    var errorResponse = new PostSubmitErrorResponse()
                    {
                        ProcessResults = processResults
                    };

                    var response = new OrderSubmitResponse()
                    {
                        HttpStatusCode = 500,
                        /*
                         * consider changing this to an xp value, could then deserialize on the front end
                         * know which integrations failed on the ui, and retrigger when the issue is resolved 
                         * through the ui potentially
                         */
                         xp = errorResponse
                    };
                    return response;
                }
                else
                {
                    var response = new OrderSubmitResponse()
                    {
                        HttpStatusCode = 200,
                    };
                    return response;
                }
            } catch (Exception ex)
            {
                // handle failure in the order failure handling process, please don't get here
                return new OrderSubmitResponse()
                {
                    HttpStatusCode = 500,
                    UnhandledErrorBody = JsonConvert.SerializeObject(ex),
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

            await Throttler.RunAsync(orderInfos, 100, 3, (orderInfo) =>
            {
                return _oc.Orders.PatchAsync(orderInfo.Item1, orderInfo.Item2, partialOrder);
            });

        }

        private bool IsStandardOrder(MarketplaceOrderWorksheet marketplaceOrderWorkSheet)
        {
            return marketplaceOrderWorkSheet.Order.xp == null || marketplaceOrderWorkSheet.Order.xp.OrderType != OrderType.Quote;
        }

        private async Task HandleZohoIntegration(List<MarketplaceOrder> updatedSupplierOrders, MarketplaceOrderWorksheet updatedMarketplaceOrderWorksheet)
        {
            var zoho_salesorder = await _zoho.CreateSalesOrder(updatedMarketplaceOrderWorksheet);
            await _zoho.CreatePurchaseOrder(zoho_salesorder, updatedSupplierOrders);
        }

        private async Task<ProcessResult> SafeIntegrationCall(ProcessType processType, Func<Task> func)
        {
            try
            {
                await func();
                return new ProcessResult()
                {
                    Failed = false,
                    Type = processType,
                };
            } catch (Exception ex)
            {
                return new ProcessResult()
                {
                    Failed = true,
                    Type = processType,
                    ErrorDetail = ex
                };
            }
        }

        private async Task<Tuple<List<MarketplaceOrder>, MarketplaceOrderWorksheet>> HandlingForwarding(MarketplaceOrderWorksheet orderWorksheet)
        {
            // forwarding
            var buyerOrder = orderWorksheet.Order;
            var orderSplitResult = await _oc.Orders.ForwardAsync(OrderDirection.Incoming, buyerOrder.ID);
            var supplierOrders = orderSplitResult.OutgoingOrders.ToList();

            // creating relationship between the buyer order and the supplier order
            // no relationship exists currently in the platform
            var updatedSupplierOrders = await CreateOrderRelationshipsAndTransferXP(buyerOrder, supplierOrders);

            // leaving this in until the sdk supports type parameters on order worksheet
            // need to get fresh order worksheet because this process has changed things about the worksheet
            var updatedWorksheet = await _ocSandboxService.GetOrderWorksheetAsync(OrderDirection.Incoming, buyerOrder.ID);
            return new Tuple<List<MarketplaceOrder>, MarketplaceOrderWorksheet>(updatedSupplierOrders, updatedWorksheet);
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
                var supplier = await _oc.Suppliers.GetAsync(supplierID);
                var supplierOrderPatch = new PartialOrder()
                {
                    ID = $"{buyerOrder.ID}-{supplierID}",
                    xp = GetNewOrderXP(buyerOrder, supplier, shipFromAddressIDsForSupplierOrder)
                };
                var updatedSupplierOrder = await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Outgoing, supplierOrder.ID, supplierOrderPatch);
                updatedSupplierOrders.Add(updatedSupplierOrder);
            }

            await _orderCommand.PatchLineItemStatus(buyerOrder.ID, LineItemStatus.Submitted);

            var buyerOrderPatch = new PartialOrder()
            {
                xp = new
                {
                    ShipFromAddressIDs = shipFromAddressIDs,
                    SupplierIDs = supplierIDs,
                    ClaimStatus = ClaimStatus.NoClaim,
                    ShippingStatus = ShippingStatus.Processing
                }
            };
            
            await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrder.ID, buyerOrderPatch);
            return updatedSupplierOrders;
        }

        private OrderXp GetNewOrderXP(MarketplaceOrder buyerOrder, Supplier supplier, List<string> shipFromAddressIDsForSupplierOrder)
        {
            var supplierOrderXp = new OrderXp()
            {
                ShipFromAddressIDs = shipFromAddressIDsForSupplierOrder,
                SupplierIDs = new List<string>() { supplier.ID },
                StopShipSync = false,
                OrderType = buyerOrder.xp.OrderType,
                QuoteOrderInfo = buyerOrder.xp.QuoteOrderInfo,
				Currency = supplier.xp.Currency
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

        private async Task ImportSupplierOrdersIntoFreightPop(IList<MarketplaceOrder> supplierOrders)
        {
            foreach (var supplierOrder in supplierOrders)
            {
                await ImportSupplierOrderIntoFreightPop(supplierOrder);
            }
        }

        private async Task ImportSupplierOrderIntoFreightPop(MarketplaceOrder supplierOrder)
        {

            var lineItems = await _oc.LineItems.ListAsync<MarketplaceLineItem>(OrderDirection.Outgoing, supplierOrder.ID);
            var firstLineItemOfSupplierOrder = lineItems.Items.First();
            var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(firstLineItemOfSupplierOrder.SupplierID);

            if (supplier.xp.SyncFreightPop)
            {
                // we further split the supplier order into multiple orders for each shipfromaddressID before it goes into freightpop
                var freightPopOrders = lineItems.Items.GroupBy(li => li.ShipFromAddressID);

                var freightPopOrderIDs = new List<string>();
                foreach (var lineItemGrouping in freightPopOrders)
                {
                    var firstLineItem = lineItemGrouping.First();

                    var freightPopOrderID = $"{supplierOrder.ID.Split('-').First()}-{firstLineItem.ShipFromAddressID}";
                    freightPopOrderIDs.Add(freightPopOrderID);

                    var supplierAddress = await _oc.SupplierAddresses.GetAsync(supplier.ID, firstLineItem.ShipFromAddressID);
                    var freightPopOrderRequest = OrderRequestMapper.Map(supplierOrder, lineItemGrouping.ToList(), supplier, supplierAddress, freightPopOrderID);
                    await _freightPopService.ImportOrderAsync(freightPopOrderRequest);
                }
            }
        }
    };
}