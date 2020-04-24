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
using Marketplace.Common.Services.FreightPop.Models;

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
                
                // because we currently only have one freightpop account all shipments for all suppliers will be returned here
                supplierWorkItem.ShipmentsToSync = await context.CallActivityAsync<List<ShipmentDetails>>("GetFreightPopShipments", 3);

                // filtering out shipments for other suppliers to simulate syncing from a supplier specific account
                supplierWorkItem.ShipmentsToSync = supplierWorkItem.ShipmentsToSync.Where(shipment => shipment.Reference1.Split('-')[1] == supplier.ID).ToList();

                foreach (var shipment in supplierWorkItem.ShipmentsToSync)
                {
                    var shipmentWorkItem = new ShipmentWorkItem()
                    {
                        Supplier = supplier,
                        SupplierToken = supplierToken,
                        Shipment = shipment
                    };
                    await context.CallActivityAsync<List<MarketplaceOrder>>("CreateShipmentsInOrderCloudIfNeeded", shipmentWorkItem);

                }
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Supplier,
                    Level = LogLevel.Success,
                    Action = Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.AuthenticateSupplierError,
                    Message = $"Supplier {supplierWorkItem.Supplier.ID} completed. FreightPopShipments: {supplierWorkItem.ShipmentsToSync.Count()}",
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

        [FunctionName("GetFreightPopShipments")]
        public async Task<List<ShipmentDetails>> GetFreightPopShipments([ActivityTrigger] int numberOfDaysBack) => await _orderOrchestrationCommand.GetFreightPopShipments(numberOfDaysBack);

        [FunctionName("CreateShipmentsInOrderCloudIfNeeded")]
        public async Task CreateShipmentsInOrderCloudIfNeeded([ActivityTrigger] ShipmentWorkItem workItem) => await _orderOrchestrationCommand.CreateShipmentsInOrderCloudIfNeeded(workItem);
        


    }
}