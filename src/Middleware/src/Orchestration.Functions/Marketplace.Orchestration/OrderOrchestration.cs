using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Commands;
using Marketplace.Common.Queries;
using System;
using OrderCloud.SDK;
using Marketplace.Common;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using System.Collections.Generic;
using Marketplace.Common.Services.ShippingIntegration.Models;
using System.Linq;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Action = Marketplace.Common.Models.Action;
using LogLevel = Marketplace.Common.Models.LogLevel;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Models.Misc;

namespace Marketplace.Orchestration
{
    public class OrderOrchestration
    {
        private IOrderOrchestrationCommand _orderOrchestrationCommand;
        private readonly LogQuery _log;
        private readonly AppSettings _appSettings;
        private SupplierShipmentSyncWorkItem _supplierWorkItem;

        public OrderOrchestration(IOrderOrchestrationCommand orderOrchestrationCommand, AppSettings appSettings, ISyncCommand sync, LogQuery log)
        {
            _orderOrchestrationCommand = orderOrchestrationCommand;
            _log = log;
            _appSettings = appSettings;
        }

        [FunctionName("ShipmentSyncOrchestration")]
        public async Task RunShipmentSyncOrchestration([OrchestrationTrigger] DurableOrchestrationContext context, ILogger logger)
        {
            try
            {
                var supplier = context.GetInput<MarketplaceSupplier>();
                var supplierToken = await context.CallActivityAsync<string>("GetAccessToken", supplier.xp.ApiClientID);
                var supplierWorkItem = new SupplierShipmentSyncWorkItem()
                {
                    Supplier = supplier,
                    SupplierToken = supplierToken,
                };
                _supplierWorkItem = supplierWorkItem;
                supplierWorkItem.OrdersToSync = await context.CallActivityAsync<List<MarketplaceOrder>>("GetOrdersNeedingShipment", supplierWorkItem);
                supplierWorkItem.ShipmentSyncOrders = await context.CallActivityAsync<List<ShipmentSyncOrder>>("GetShipmentSyncOrders", supplierWorkItem);
                supplierWorkItem.ShipmentSyncOrders = await context.CallActivityAsync<List<ShipmentSyncOrder>>("GetShipmentDetailsForShipmentSyncOrders", supplierWorkItem);
                var shipmentSyncOrdersToUpdate = supplierWorkItem.ShipmentSyncOrders.Where(s => s.FreightPopShipmentResponses != null && s.FreightPopShipmentResponses.Data != null);
                foreach (var shipmentSyncOrderToUpdate in shipmentSyncOrdersToUpdate)
                {
                    var orderWorkItem = new OrderWorkItem()
                    {
                        Supplier = supplier,
                        SupplierToken = supplierToken,
                        ShipmentSyncOrder = shipmentSyncOrderToUpdate
                    };
                    await context.CallActivityAsync<List<MarketplaceOrder>>("CreateShipmentsInOrderCloudIfNeeded", orderWorkItem);

                }
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Supplier,
                    Level = LogLevel.Success,
                    Action = Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.AuthenticateSupplierError,
                    Message = $"Supplier {supplierWorkItem.Supplier.ID} completed. OrderCloud Orders: {supplierWorkItem.OrdersToSync.Count()}. FreightPOP Orders To Update: {shipmentSyncOrdersToUpdate.Count()}",
                });
            }
            catch (OrchestrationException ex)
            {
                logger.LogError($"{ex.Error.Type}: {ex.Message}", ex.Error.Data);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error syncing orders for supplier: {_supplierWorkItem.Supplier.ID}, error: {ex.Message}");
            } 
        }

        [FunctionName("GetAccessToken")]
        public async Task<string> GetAccessToken([ActivityTrigger] string apiClientID) => await _orderOrchestrationCommand.GetAccessTokenAsync(apiClientID);

        [FunctionName("GetOrdersNeedingShipment")]
        public async Task<List<MarketplaceOrder>> GetOrdersNeedingShipment([ActivityTrigger] SupplierShipmentSyncWorkItem workItem) => await _orderOrchestrationCommand.GetOrdersNeedingShipmentAsync(workItem);

        [FunctionName("GetShipmentSyncOrders")]
        public async Task<List<ShipmentSyncOrder>> GetShipmentSyncOrders([ActivityTrigger] SupplierShipmentSyncWorkItem workItem) => await _orderOrchestrationCommand.GetShipmentSyncOrders(workItem);

        [FunctionName("GetShipmentDetailsForShipmentSyncOrders")]
        public async Task<List<ShipmentSyncOrder>> GetShipmentDetailsForShipmentSyncOrders([ActivityTrigger] SupplierShipmentSyncWorkItem workItem) => await _orderOrchestrationCommand.GetShipmentDetailsForShipmentSyncOrders(workItem);

        [FunctionName("CreateShipmentsInOrderCloudIfNeeded")]
        public async Task CreateShipmentsInOrderCloudIfNeeded([ActivityTrigger] OrderWorkItem workItem) => await _orderOrchestrationCommand.CreateShipmentsInOrderCloudIfNeeded(workItem);


    }
}