using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using OrderCloud.SDK;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models.Marketplace;
using Marketplace.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.avalara;
using ordercloud.integrations.library;
using Marketplace.Models.Extended;
using Npoi.Mapper;

namespace Marketplace.Common.Commands
{
    public interface IPostSubmitCommand
    {
        Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet order);
        Task<OrderSubmitResponse> HandleZohoRetry(string orderID, VerifiedUserContext user);
    }

    public class PostSubmitCommand : IPostSubmitCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly IZohoCommand _zoho;
        private readonly IAvalaraCommand _avalara;
        private readonly ISendgridService _sendgridService;
        private readonly ILineItemCommand _lineItemCommand;

        public PostSubmitCommand(ISendgridService sendgridService, IAvalaraCommand avatax, IOrderCloudClient oc, IZohoCommand zoho, ILineItemCommand lineItemCommand)
        {
            _oc = oc;
            _avalara = avatax;
            _zoho = zoho;
            _sendgridService = sendgridService;
            _lineItemCommand = lineItemCommand;
        }

        public async Task<OrderSubmitResponse> HandleZohoRetry(string orderID, VerifiedUserContext user)
        {
            var worksheet = await _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderID, user.AccessToken);
            var supplierOrders = await Throttler.RunAsync(worksheet.LineItems.GroupBy(g => g.SupplierID).Select(s => s.Key), 100, 10, item => _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Outgoing,
                $"{worksheet.Order.ID}-{item}", user.AccessToken));

            return await CreateOrderSubmitResponse(
                new List<ProcessResult>() { await this.PerformZohoTasks(worksheet, supplierOrders) }, 
                new List<MarketplaceOrder> { worksheet.Order });
        }

        private async Task<ProcessResult> PerformZohoTasks(MarketplaceOrderWorksheet worksheet, IList<MarketplaceOrder> supplierOrders)
        {
            var (salesAction, zohoSalesOrder) = await ProcessActivityCall(
                ProcessType.Accounting,
                "Create Zoho Sales Order",
                _zoho.CreateSalesOrder(worksheet));

            var (poAction, zohoPurchaseOrder) = await ProcessActivityCall(
                ProcessType.Accounting,
                "Create Zoho Purchase Order",
                _zoho.CreateOrUpdatePurchaseOrder(zohoSalesOrder, supplierOrders.ToList()));

            var (shippingAction, zohoShippingOrder) = await ProcessActivityCall(
                ProcessType.Accounting,
                "Create Zoho Shipping Purchase Order",
                _zoho.CreateShippingPurchaseOrder(zohoSalesOrder, worksheet));
            return new ProcessResult()
            {
                Type = ProcessType.Accounting,
                Activity = new List<ProcessResultAction>() {salesAction, poAction, shippingAction}
            };
        }

        public async Task<OrderSubmitResponse> HandleBuyerOrderSubmit(MarketplaceOrderWorksheet orderWorksheet)
        {
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

        private async Task<List<ProcessResult>> HandleIntegrations(List<MarketplaceOrder> supplierOrders, MarketplaceOrderWorksheet orderWorksheet)
        {
            // STEP 1: SendGrid notifications
            var results = new List<ProcessResult>();

            var notifications = await ProcessActivityCall(
                ProcessType.Notification,
                "Sending Order Submit Emails",
                _sendgridService.SendOrderSubmitEmail(orderWorksheet));
            results.Add(new ProcessResult()
            {
                Type = ProcessType.Notification,
                Activity = new List<ProcessResultAction>() { notifications }
            });

            if (!orderWorksheet.IsStandardOrder())
                return results;

            // STEP 2: Avalara tax transaction
            var tax = await ProcessActivityCall(
                ProcessType.Tax,
                "Creating Tax Transaction",
                HandleTaxTransactionCreationAsync(orderWorksheet.Reserialize<OrderWorksheet>()));
            results.Add(new ProcessResult()
            {
                Type = ProcessType.Tax,
                Activity = new List<ProcessResultAction>() { tax }
            });

            // STEP 3: Zoho orders
            results.Add(await this.PerformZohoTasks(orderWorksheet, supplierOrders));

            // STEP 4: Validate shipping
            var shipping = await ProcessActivityCall(
                ProcessType.Shipping,
                "Validate Shipping",
                ValidateShipping(orderWorksheet));
            results.Add(new ProcessResult()
            {
                Type = ProcessType.Shipping,
                Activity = new List<ProcessResultAction>() { shipping }
            });

            return results;
        }
        
        private async Task<OrderSubmitResponse> CreateOrderSubmitResponse(List<ProcessResult> processResults, List<MarketplaceOrder> ordersRelatingToProcess)
        {
            try
            {
                if (processResults.All(i => i.Activity.All(a => a.Success)))
                {
                    return new OrderSubmitResponse()
                    {
                        HttpStatusCode = 200,
                        xp = new OrderSubmitResponseXp()
                        {
                            ProcessResults = processResults
                        }
                    };
                }
                    
                await MarkOrdersAsNeedingAttention(ordersRelatingToProcess); 
                return new OrderSubmitResponse()
                {
                    HttpStatusCode = 500,
                    xp = new OrderSubmitResponseXp()
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
            catch (OrderCloudIntegrationException integrationEx)
            {
                return new ProcessResultAction()
                {
                    Description = description,
                    ProcessType = type,
                    Success = false,
                    Exception = new ProcessResultException(integrationEx)
                };
            }
            catch (FlurlHttpException flurlEx)
            {
                return new ProcessResultAction()
                {
                    Description = description,
                    ProcessType = type,
                    Success = false,
                    Exception = new ProcessResultException(flurlEx)
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
            catch (OrderCloudIntegrationException integrationEx)
            {
                return new Tuple<ProcessResultAction, T>(new ProcessResultAction()
                {
                    Description = description,
                    ProcessType = type,
                    Success = false,
                    Exception = new ProcessResultException(integrationEx)
                }, new T());
            }
            catch (FlurlHttpException flurlEx)
            {
                return new Tuple<ProcessResultAction, T>(new ProcessResultAction()
                {
                    Description = description,
                    ProcessType = type,
                    Success = false,
                    Exception = new ProcessResultException(flurlEx)
                }, new T());
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

        private async Task<Tuple<List<MarketplaceOrder>, MarketplaceOrderWorksheet, List<ProcessResultAction>>> HandlingForwarding(MarketplaceOrderWorksheet orderWorksheet)
        {
            var activities = new List<ProcessResultAction>();
            // forwarding
            var (forwardAction, forwardedOrders) = await ProcessActivityCall(
                ProcessType.Forwarding,
                "OrderCloud API Order.ForwardAsync",
                _oc.Orders.ForwardAsync(OrderDirection.Incoming, orderWorksheet.Order.ID)
            );
            activities.Add(forwardAction);

            var supplierOrders = forwardedOrders.OutgoingOrders.ToList();

            // creating relationship between the buyer order and the supplier order
            // no relationship exists currently in the platform
            var (updateAction, marketplaceOrders) = await ProcessActivityCall(
                ProcessType.Forwarding, "Create Order Relationships And Transfer XP",
                CreateOrderRelationshipsAndTransferXP(orderWorksheet, supplierOrders));
            activities.Add(updateAction);

            // need to get fresh order worksheet because this process has changed things about the worksheet
            var (getAction, marketplaceOrderWorksheet) = await ProcessActivityCall(
                ProcessType.Forwarding, 
                "Get Updated Order Worksheet",
                _oc.IntegrationEvents.GetWorksheetAsync<MarketplaceOrderWorksheet>(OrderDirection.Incoming, orderWorksheet.Order.ID));
            activities.Add(getAction);

            return await Task.FromResult(new Tuple<List<MarketplaceOrder>, MarketplaceOrderWorksheet, List<ProcessResultAction>>(marketplaceOrders, marketplaceOrderWorksheet, activities));
        }

        //TODO: probably want to move this to a command so it's isolated and testable
        private async Task<List<MarketplaceOrder>> CreateOrderRelationshipsAndTransferXP(MarketplaceOrderWorksheet buyerOrder, List<Order> supplierOrders)
        {
            var updatedSupplierOrders = new List<MarketplaceOrder>();
            var supplierIDs = new List<string>();
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, buyerOrder.Order.ID);
            var shipFromAddressIDs = lineItems.Items.DistinctBy(li => li.ShipFromAddressID).Select(li => li.ShipFromAddressID).ToList();

            foreach (var supplierOrder in supplierOrders)
            {
                supplierIDs.Add(supplierOrder.ToCompanyID);
                var shipFromAddressIDsForSupplierOrder = shipFromAddressIDs?.Where(addressID => addressID != null && addressID.Contains(supplierOrder.ToCompanyID)).ToList();
                var supplier = await _oc.Suppliers.GetAsync<MarketplaceSupplier>(supplierOrder.ToCompanyID);
                var suppliersShipEstimates = buyerOrder.ShipEstimateResponse?.ShipEstimates?.Where(se => se.xp.SupplierID == supplier.ID);
                var supplierOrderPatch = new PartialOrder() {
                    ID = $"{buyerOrder.Order.ID}-{supplierOrder.ToCompanyID}",
                    xp = new OrderXp() {
                        ShipFromAddressIDs = shipFromAddressIDsForSupplierOrder,
                        SupplierIDs = new List<string>() { supplier.ID },
                        StopShipSync = false,
                        OrderType = buyerOrder.Order.xp.OrderType,
                        QuoteOrderInfo = buyerOrder.Order.xp.QuoteOrderInfo,
                        Currency = supplier.xp.Currency,
                        ClaimStatus = ClaimStatus.NoClaim,
                        ShippingStatus = ShippingStatus.Processing,
                        SubmittedOrderStatus = SubmittedOrderStatus.Open,
                        SelectedShipMethodsSupplierView = suppliersShipEstimates != null ? MapSelectedShipMethod(suppliersShipEstimates) : null
                    }
                };
                var updatedSupplierOrder = await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Outgoing, supplierOrder.ID, supplierOrderPatch);
                updatedSupplierOrders.Add(updatedSupplierOrder);
            }

            await _lineItemCommand.SetInitialSubmittedLineItemStatuses(buyerOrder.Order.ID);

            var buyerOrderPatch = new PartialOrder() {
                xp = new {
                    ShipFromAddressIDs = shipFromAddressIDs,
                    SupplierIDs = supplierIDs,
                    ClaimStatus = ClaimStatus.NoClaim,
                    ShippingStatus = ShippingStatus.Processing,
                    SubmittedOrderStatus = SubmittedOrderStatus.Open
                }
            };

            await _oc.Orders.PatchAsync(OrderDirection.Incoming, buyerOrder.Order.ID, buyerOrderPatch);
            return updatedSupplierOrders;
        }

        private List<ShipMethodSupplierView> MapSelectedShipMethod(IEnumerable<ShipEstimate> shipEstimates)
		{
            var selectedShipMethods = shipEstimates.Select(se =>
            {
                var selected = se.ShipMethods.First(sm => sm.ID == se.SelectedShipMethodID);
                return new ShipMethodSupplierView()
                {
                    EstimatedTransitDays = selected.EstimatedTransitDays,
                    Name = selected.Name,
                    ShipFromAddressID = se.xp.ShipFromAddressID
                };
            }).ToList();
            return selectedShipMethods;
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

        private async Task ValidateShipping(OrderWorksheet orderWorksheet)
        {
            if(orderWorksheet.ShipEstimateResponse.HttpStatusCode != 200)
            {
                throw new Exception(orderWorksheet.ShipEstimateResponse.UnhandledErrorBody);
            }
            if(orderWorksheet.ShipEstimateResponse.ShipEstimates.Any(s => s.SelectedShipMethodID == "NO_SHIPPING_RATES"))
            {
                throw new Exception("No shipping rates could be determined - fallback shipping rate of $20 3-day was used");
            }
        }
    };
}