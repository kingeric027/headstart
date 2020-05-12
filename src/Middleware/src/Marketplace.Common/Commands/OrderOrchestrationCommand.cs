using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using System.Linq;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using System;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Models.Models.Misc;
using ordercloud.integrations.extensions;
using Action = System.Action;

namespace Marketplace.Common.Commands
{
    public interface IOrderOrchestrationCommand
    {
        Task<string> GetAccessTokenAsync(string apiClientID);
        Task<List<MarketplaceSupplier>> GetSuppliersNeedingSync();
        Task<List<ShipmentDetails>> GetFreightPopShipments(int numOfDaysBack);
        Task CreateShipmentsInOrderCloudIfNeeded(ShipmentWorkItem shipmentWorkItem);
    }

    public class OrderOrchestrationCommand : IOrderOrchestrationCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly string _supplierID;
        private IFreightPopService _freightPopService;
        private readonly AppSettings _appSettings;
        private readonly LogQuery _log;

        public OrderOrchestrationCommand(LogQuery log, IFreightPopService freightPopService, AppSettings appSettings, string supplierID)
        {
			_oc = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = appSettings.OrderCloudSettings.ApiUrl,
                AuthUrl = appSettings.OrderCloudSettings.AuthUrl,
                ClientId = appSettings.OrderCloudSettings.ClientID,
                ClientSecret = appSettings.OrderCloudSettings.ClientSecret,
                Roles = new[]
                {
                    ApiRole.FullAccess
                }
            });
            _freightPopService = freightPopService;
            _appSettings = appSettings;
            _supplierID = supplierID;
            _log = log;
        }

        public async Task<string> GetAccessTokenAsync(string apiClientID)
        {
            try { 
                var supplierOrderCloudClient = new OrderCloudClient(new OrderCloudClientConfig
                {
                    ApiUrl = _appSettings.OrderCloudSettings.ApiUrl,
                    AuthUrl = _appSettings.OrderCloudSettings.AuthUrl,
                    ClientId = apiClientID,
                    ClientSecret = _appSettings.OrderCloudSettings.ClientSecret,
                    Roles = new[]
                               {
                                     ApiRole.FullAccess
                                }
                });
                await supplierOrderCloudClient.AuthenticateAsync();
                var supplierToken = supplierOrderCloudClient.TokenResponse.AccessToken;
                return supplierToken;
            } catch (OrderCloudException ex)
            {
                await _log.Save(new OrchestrationLog(ex)
                {
                    RecordType = RecordType.Supplier,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.AuthenticateSupplierError,
                    Message = $"Failed to authenticate supplier with ApiClientID {apiClientID}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.AuthenticateSupplierError, $"Error authenticating for ApiClient {apiClientID}");
            }
        } 

        public async Task<List<MarketplaceSupplier>> GetSuppliersNeedingSync()
        {
            // todo support over 100 suppliers
            var suppliers = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(pageSize: 100, filters: $"xp.SyncFreightPop=true");
            return suppliers.Items.ToList();
        }

        public async Task CreateShipmentsInOrderCloudIfNeeded(ShipmentWorkItem shipmentWorkItem)
        {
            try
            {
                var orderCloudOrderID = await ValidAndGetOrderCloudOrder(shipmentWorkItem);
                var buyerIDForOrder = await GetBuyerIDForSupplierOrder(orderCloudOrderID);
                 await AddShipmentToOrderCloud(shipmentWorkItem, orderCloudOrderID, buyerIDForOrder);
            } catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Order,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded,
                    Message = $"Error creating ordercloud shipment for freightpop order id: {shipmentWorkItem.Shipment.ShipmentId}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded, $"Error creating ordercloud shipment for freightpop order id: {shipmentWorkItem.Shipment.ShipmentId}. Error: {ex.Message}");
            }
        }

        private async Task<string> ValidAndGetOrderCloudOrder(ShipmentWorkItem shipmentWorkItem)
        {
            var ref1 = shipmentWorkItem.Shipment.Reference1;
            var potentialOrderCloudOrderID = "";
            if (ref1 != null && ref1.Length > 0)
            {
                var refPieces = ref1.Split('-');
                if(refPieces.Length == 3 || refPieces.Length == 2)
                {
                    // this will recognize a reference number for a FreightPOP order (OC order to a specific supplier location)
                    // or an othercloud order
                    potentialOrderCloudOrderID = refPieces[0] + '-' + refPieces[1];
                } else
                {
                    throw new OrchestrationException(OrchestrationErrorType.NoRelatedOrderCloudOrderFound, $"Could not find ordercloud order for freightpop ref1: {ref1}.");
                }
            }
            try
            {
                await _oc.Orders.GetAsync(OrderDirection.Incoming, potentialOrderCloudOrderID, shipmentWorkItem.SupplierToken);
            } catch (Exception ex)
            {
                throw new OrchestrationException(OrchestrationErrorType.NoRelatedOrderCloudOrderFound, $"Could not find ordercloud order for freightpop ref1: {ref1}. Error: {ex.Message}");
            }
            return potentialOrderCloudOrderID;
        }

        public async Task<List<ShipmentDetails>> GetFreightPopShipments(int numOfDaysBack)
        {
            try
            {
                var daysToGetShipments = new List<int> { };
                for (var i = 0; i < numOfDaysBack; i++)
                {
                    daysToGetShipments.Add(i);
                }
                var shipmentDetailsResponses = await Throttler.RunAsync(daysToGetShipments, 100, 2, (daysBack) =>
                {
                    return _freightPopService.GetShipmentsByDate(daysBack);
                });
                // sometimes the values returned here are null for the response object which is not exactly what I would expect when there are no shipments
                // regardless we are filtering those out
                shipmentDetailsResponses = shipmentDetailsResponses.Where(shipmentDetailResponse => shipmentDetailResponse != null && shipmentDetailResponse.Data != null).ToList();
                if(shipmentDetailsResponses.Count() > 0)
                {
                    var shipmentsFromPastDays = shipmentDetailsResponses.SelectMany(response => response.Data).ToList();
                    return shipmentsFromPastDays;
                } else
                {
                    return new List<ShipmentDetails> { };
                }
            }
            catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Order,
                    Action = Marketplace.Common.Models.Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.GetFreightPopShipments,
                    Message = $"Error retrieving shipments from FreightPOP. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.GetFreightPopShipments, $"Error retrieving shipments from FreightPOP. Error {ex.Message}");
            }
        }
        
        private async Task AddShipmentToOrderCloud(ShipmentWorkItem shipmentWorkItem, string supplierOrderID, string buyerID)
        {
            try
            {
                var ocShipment = await _oc.Shipments.CreateAsync(OCShipmentMapper.Map(shipmentWorkItem.Shipment, buyerID), accessToken: shipmentWorkItem.SupplierToken);

                await Throttler.RunAsync(shipmentWorkItem.Shipment.Items, 100, 1,
                (freightPopShipmentItem) =>
                {
                // currently this creates two items in ordercloud inadvertantely, platform bug is being resolved
                    return _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(freightPopShipmentItem, supplierOrderID), accessToken: shipmentWorkItem.SupplierToken);
                });
            } catch (Exception ex)
            {
                if(ex.Message == "IdExists: Object already exists.")
                {
                    await _log.Save(new OrchestrationLog()
                    {
                        RecordType = RecordType.Order,
                        Level = LogLevel.Progress,
                        Action = Marketplace.Common.Models.Action.SyncShipments,
                        Message = $"Shipment already created: {shipmentWorkItem.Shipment.ShipmentId}",
                    });
                }
                else
                {
                    throw new OrchestrationException(OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded, $"Error creating shipment in ordercloud. Error {ex.Message}");
                }
            }
        }

        private async Task<string> GetBuyerIDForSupplierOrder(string orderCloudSupplierOrderID)
        {
            var buyerOrderID = orderCloudSupplierOrderID.Split("-").First();
            var relatedBuyerOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, buyerOrderID);
            return relatedBuyerOrder.FromCompanyID;
        }

        private async Task<ListPage<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(int page, string supplierToken)
        {
            var orders = await _oc.Orders.ListAsync<MarketplaceOrder>(OrderDirection.Incoming, page: page, pageSize: 100, filters: $"IsSubmitted=true&Status=Open&xp.StopShipSync=false", accessToken: supplierToken);
            if(orders.Meta.TotalCount > 0)
            {
                return orders;
            }
            return orders;
        }


    }
}
