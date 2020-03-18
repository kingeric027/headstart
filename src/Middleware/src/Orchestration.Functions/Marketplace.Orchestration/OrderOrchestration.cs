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

namespace Marketplace.Orchestration
{
    public class OrderOrchestration
    {
        private IOrderOrchestrationCommand _orderOrchestrationCommand;
        private readonly LogQuery _log;
        private readonly AppSettings _appSettings;

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
                var supplierToken = context.GetInput<string>();
                var orders = await context.CallActivityAsync<List<MarketplaceOrder>>("GetOrdersNeedingShipment", supplierToken);
                var shipmentSyncOrders = await context.CallActivityAsync<List<ShipmentSyncOrder>>("GetShipmentSyncOrders", orders);
                var shipmentSyncOrdersWithFreightPopResponses = await context.CallActivityAsync<List<ShipmentSyncOrder>>("GetShipmentDetailsForShipmentSyncOrders", shipmentSyncOrders);
                var shipmentSyncOrdersToUpdate = shipmentSyncOrdersWithFreightPopResponses.Where(s => s.FreightPopShipmentResponses != null && s.FreightPopShipmentResponses.Data != null);
                foreach (var shipmentSyncOrderToUpdate in shipmentSyncOrdersToUpdate)
                {
                    await context.CallActivityAsync<List<MarketplaceOrder>>("CreateShipmentsInOrderCloudIfNeeded", (shipmentSyncOrderToUpdate, supplierToken));

                }
            } catch (Exception ex)
            {
                logger.LogError($"Error syncing orders for supplier: {_orderOrchestrationCommand.SupplierID()}, error: {ex.Message}");
            }
        }

        [FunctionName("GetOrdersNeedingShipment")]
        public async Task<List<MarketplaceOrder>> GetOrdersNeedingShipment([ActivityTrigger] string supplierToken) => await _orderOrchestrationCommand.GetOrdersNeedingShipmentAsync(supplierToken);

        [FunctionName("GetShipmentSyncOrders")]
        public async Task<List<ShipmentSyncOrder>> GetShipmentSyncOrders([ActivityTrigger] List<MarketplaceOrder> orders) => await _orderOrchestrationCommand.GetShipmentSyncOrders(orders);

        [FunctionName("GetShipmentDetailsForShipmentSyncOrders")]
        public async Task<List<ShipmentSyncOrder>> GetShipmentDetailsForShipmentSyncOrders([ActivityTrigger] List<ShipmentSyncOrder> shipmentSyncOrders) => await _orderOrchestrationCommand.GetShipmentDetailsForShipmentSyncOrders(shipmentSyncOrders);

        [FunctionName("CreateShipmentsInOrderCloudIfNeeded")]
        public async Task CreateShipmentsInOrderCloudIfNeeded([ActivityTrigger] Tuple<ShipmentSyncOrder, string> input)
        {
            var(shipmentSyncOrder, supplierToken) = input;
            await _orderOrchestrationCommand.CreateShipmentsInOrderCloudIfNeeded(shipmentSyncOrder, supplierToken);
        }


    }
}