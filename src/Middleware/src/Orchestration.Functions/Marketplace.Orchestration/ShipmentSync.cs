using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Commands;
using Marketplace.Common.Queries;
using Marketplace.Common;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Action = Marketplace.Common.Models.Action;
using LogLevel = Marketplace.Common.Models.LogLevel;
using Marketplace.Models.Models.Misc;
using ordercloud.integrations.freightpop;

namespace Marketplace.Orchestration
{
    public class ShipmentSync
    {
        private readonly IOrderOrchestrationCommand _orderOrchestrationCommand;
        private readonly LogQuery _log;
        private readonly AppSettings _appSettings;
        private SupplierShipmentSyncWorkItem _supplierWorkItem;

        public ShipmentSync(IOrderOrchestrationCommand orderOrchestrationCommand, AppSettings appSettings, ISyncCommand sync, LogQuery log)
        {
            _orderOrchestrationCommand = orderOrchestrationCommand;
            _log = log;
            _appSettings = appSettings;
        }

        [FunctionName("FreightPOPUICreatedShipmentsSupplierSync")]
        public async Task RunShipmentSyncOrchestrationForSupplier([OrchestrationTrigger] DurableOrchestrationContext context, ILogger logger)
        {
            try
            {
                var supplier = context.GetInput<MarketplaceSupplier>();
                var supplierOCToken = await context.CallActivityAsync<string>("GetOCAccessToken", supplier.xp.ApiClientID);
                var freightPopToken = await context.CallActivityAsync<string>("GetFreightPopToken", supplier.ID);
                var supplierWorkItem = new SupplierShipmentSyncWorkItem()
                {
                    Supplier = supplier,
                    SupplierToken = supplierOCToken,
                };
                _supplierWorkItem = supplierWorkItem;

                // because we currently only have one freightpop account all shipments for all suppliers will be returned here
                supplierWorkItem.ShipmentsToSync = await context.CallActivityAsync<List<ShipmentDetails>>("GetFreightPopShipmentsForSupplier", freightPopToken);

                // when we have multiple accounts for different suppliers these shipment would just be for a given supplier
                // filtering out shipments for other suppliers to simulate syncing from a supplier specific account
                supplierWorkItem.ShipmentsToSync = supplierWorkItem.ShipmentsToSync.Where(shipment => shipment.Reference1.Split('-')[1] == supplier.ID).ToList();


                foreach (var shipment in supplierWorkItem.ShipmentsToSync)
                {
                    var shipmentWorkItem = new ShipmentWorkItem()
                    {
                        Supplier = supplier,
                        SupplierOCToken = supplierOCToken,
                        Shipment = shipment
                    };
                    await context.CallActivityAsync<List<MarketplaceOrder>>("CreateShipmentsInOrderCloudIfNeededForSupplier", shipmentWorkItem);

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

        [FunctionName("FedexShipManagerCreatedShipmentSyncProcess")]
        public async Task FedexShipManagerCreatedShipmentSyncProcess([OrchestrationTrigger] DurableOrchestrationContext context, ILogger logger)
        {
            try
            {
                var freightPopToken = await context.CallActivityAsync<string>("GetFreightPopToken", "");
                var shipmentsToSync = await context.CallActivityAsync<List<ShipmentDetails>>("GetFreightPopShipmentsFromShipManager", freightPopToken);

                foreach (var shipment in shipmentsToSync)
                {
                    await context.CallActivityAsync<List<MarketplaceOrder>>("CreateShipmentsInOrderCloudIfNeededForShipManager", shipment);

                }
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Supplier,
                    Level = LogLevel.Success,
                    Action = Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.AuthenticateSupplierError,
                    Message = $"Ship Manager Created Shipment sync completed. FreightPopShipments: {shipmentsToSync.Count()}",
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

        [FunctionName("GetFreightPopToken")]
        public async Task<string> GetFreightPopToken([ActivityTrigger] string supplierID) {
            if(supplierID.Length > 0)
            {
                return await _orderOrchestrationCommand.GetFreightPopAccessTokenAsync(supplierID);
            } else
            {
                return await _orderOrchestrationCommand.GetFreightPopAccessTokenAsync();
            }
        }

        [FunctionName("GetOCAccessToken")]
        public async Task<string> GetOCAccessToken([ActivityTrigger] string apiClientID) => await _orderOrchestrationCommand.GetOCAccessTokenAsync(apiClientID);
        
        [FunctionName("GetFreightPopShipmentsForSupplier")]
        public async Task<List<ShipmentDetails>> GetFreightPopShipmentsForSupplier([ActivityTrigger] string freightPopToken) => await _orderOrchestrationCommand.GetFreightPopShipments(freightPopToken, ShipmentSyncType.SupplierFreightPopAccount);

        [FunctionName("GetFreightPopShipmentsFromShipManager")]
        public async Task<List<ShipmentDetails>> GetFreightPopShipmentsFromShipManager([ActivityTrigger] string token) => await _orderOrchestrationCommand.GetFreightPopShipments(token, ShipmentSyncType.FedexShipManager);
        
        [FunctionName("CreateShipmentsInOrderCloudIfNeededForSupplier")]
        public async Task CreateShipmentsInOrderCloudIfNeededForSupplier([ActivityTrigger] ShipmentWorkItem workItem) => await _orderOrchestrationCommand.CreateShipmentInOrderCloudIfNeeded(workItem);

        [FunctionName("CreateShipmentsInOrderCloudIfNeededForShipManager")]
        public async Task CreateShipmentsInOrderCloudIfNeededForShipManager([ActivityTrigger] ShipmentDetails shipment) => await _orderOrchestrationCommand.CreateShipmentInOrderCloudIfNeeded(shipment);
    }
}