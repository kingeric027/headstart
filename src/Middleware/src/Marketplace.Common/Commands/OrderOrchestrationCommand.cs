using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using System.Linq;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Helpers;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using System;
using Marketplace.Common.Queries;
using Marketplace.Models.Orchestration;
using DurableTask.Core.Exceptions;
using Marketplace.Models.Misc;
using Action = Marketplace.Models.Misc.Action;
using Marketplace.Models.Exceptions;
using OrchestrationException = Marketplace.Models.Exceptions.OrchestrationException;
using Marketplace.Models.Models.Misc;

namespace Marketplace.Common.Commands
{
    public interface IOrderOrchestrationCommand
    {
        Task<string> GetAccessTokenAsync(string apiClientID);
        Task<List<MarketplaceSupplier>> GetSuppliersNeedingSync();
        Task<List<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(SupplierShipmentSyncWorkItem workItem);
        Task<List<ShipmentSyncOrder>> GetShipmentSyncOrders(SupplierShipmentSyncWorkItem workItem);
        Task<List<ShipmentSyncOrder>> GetShipmentDetailsForShipmentSyncOrders(SupplierShipmentSyncWorkItem workItem);
        Task CreateShipmentsInOrderCloudIfNeeded(OrderWorkItem workItem);
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
                    Action = Action.SyncShipments,
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

        public async Task<List<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(SupplierShipmentSyncWorkItem workItem)
        {
            try
            {
                var currentPage = 1;
                var allOrders = new List<MarketplaceOrder>();

                var firstPage = await GetOrdersNeedingShipmentAsync(currentPage++, workItem.SupplierToken);
                allOrders.AddRange(firstPage.Items);

                if (firstPage.Meta.TotalPages > 0)
                {
                    var subsequentPages = await Throttler.RunAsync(new string[firstPage.Meta.TotalPages - 1], 100, 5,
                        (s) =>
                        {
                            return GetOrdersNeedingShipmentAsync(currentPage++, workItem.SupplierToken);
                        });

                    foreach (var orderListPage in subsequentPages)
                    {
                        allOrders.AddRange(orderListPage.Items);
                    }
                }

                return allOrders;
            } catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Supplier,
                    Action = Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.GetOrdersNeedingShipmentError,
                    Message = $"Failed to retrieve orders for supplier: {workItem.Supplier.ID}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.GetOrdersNeedingShipmentError, $"Error retrieving orders for {workItem.Supplier.ID}");
            }
        }

        public async Task<List<ShipmentSyncOrder>> GetShipmentSyncOrders(SupplierShipmentSyncWorkItem workItem)
        {
            try
            {
                var shipmentSyncOrders = new List<ShipmentSyncOrder>();
                foreach (var order in workItem.OrdersToSync)
                {
                    if (order.xp.ShipFromAddressIDs == null || DateTimeOffset.Now.AddDays(-15) > order.DateSubmitted)
                    {
                        // stop sync and mark as needing attention if 
                        // no freightpop order IDs on supplier order
                        // or order is over 15 days old
                        await _oc.Orders.PatchAsync(OrderDirection.Outgoing, order.ID, new PartialOrder() { xp = new { NeedsAttention = true, StopShipSync = true } });
                    }
                    else
                    {
                        foreach (var shipFromAddressID in order.xp.ShipFromAddressIDs)
                        {
                            shipmentSyncOrders.Add(new ShipmentSyncOrder()
                            {
                                OrderCloudOrderID = order.ID,
                                FreightPopOrderID = $"{order.ID.Split('-').First()}-{shipFromAddressID}",
                                Order = order,
                                FreightPopShipmentResponses = new Response<List<ShipmentDetails>>()
                            });
                        }
                    }
                }
                return shipmentSyncOrders;
            } catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Supplier,
                    Action = Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.GetShipmentSyncOrders,
                    Message = $"Error getting shipment sync orders for: {workItem.Supplier.ID}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.GetOrdersNeedingShipmentError, $"getting shipment sync orders for {workItem.Supplier.ID}");
            }
        }

        public async Task<List<ShipmentSyncOrder>> GetShipmentDetailsForShipmentSyncOrders(SupplierShipmentSyncWorkItem workItem)
        {
            try
            {
                // running slowly due to freightpop limits
                var shipmentSyncOrdersWithResponses = await Throttler.RunAsync(workItem.ShipmentSyncOrders, 2000, 1,
                 (shipmentSyncOrder) =>
                 {
                     return GetShipmentDetailsForShipmentSyncOrder(shipmentSyncOrder);
                 });
                return shipmentSyncOrdersWithResponses.ToList();
            } catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Supplier,
                    Action = Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.GetShipmentDetailsForShipmentSyncOrders,
                    Message = $"Error getting shipment sync orders for: {workItem.Supplier.ID}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.GetShipmentDetailsForShipmentSyncOrders, $"getting shipment sync orders for {workItem.Supplier.ID}");
            }
        }

        public async Task<ShipmentSyncOrder> GetShipmentDetailsForShipmentSyncOrder(ShipmentSyncOrder shipmentSyncOrder)
        {
            shipmentSyncOrder.FreightPopShipmentResponses = await _freightPopService.GetShipmentsForOrder(shipmentSyncOrder.FreightPopOrderID);
            return shipmentSyncOrder;

        }

        public async Task CreateShipmentsInOrderCloudIfNeeded(OrderWorkItem workItem)
        {
            try
            {
                await Throttler.RunAsync(workItem.ShipmentSyncOrder.FreightPopShipmentResponses.Data, 100, 1,
                (shipmentDetails) =>
                {
                    return CreateShipmentInOrderCloudIfNeeded(shipmentDetails, workItem.ShipmentSyncOrder.Order, workItem.SupplierToken);
                });
            } catch (Exception ex)
            {
                await _log.Save(new OrchestrationLog()
                {
                    RecordType = RecordType.Order,
                    Action = Action.SyncShipments,
                    ErrorType = OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded,
                    Message = $"Error creating ordercloud shipment for freightpop order id: {workItem.ShipmentSyncOrder.FreightPopOrderID}. Error: {ex.Message}",
                });
                throw new OrchestrationException(OrchestrationErrorType.CreateShipmentsInOrderCloudIfNeeded, $"Error creating ordercloud shipment for freightpop order id: {workItem.ShipmentSyncOrder.FreightPopOrderID}. Error: {ex.Message}");
            }
        }

        private async Task CreateShipmentInOrderCloudIfNeeded(ShipmentDetails freightPopShipment, MarketplaceOrder relatedOrder, string supplierToken)
        {
            var orderCloudShipments = await _oc.Shipments.ListAsync(orderID: relatedOrder.ID, accessToken: supplierToken);
            var buyerIDForOrder = await GetBuyerIDForSupplierOrder(relatedOrder);

            // needs to be more complex in the future, logic pending discussion around split of orders by supplierid
            if (!orderCloudShipments.Items.Any(o => o.ID == freightPopShipment.ShipmentId))
            {
                await AddShipmentToOrderCloud(freightPopShipment, relatedOrder, buyerIDForOrder, supplierToken);
            }
        }
        
        private async Task AddShipmentToOrderCloud(ShipmentDetails freightPopShipment, MarketplaceOrder relatedOrder, string buyerID, string supplierToken)
        {
            var ocShipment = await _oc.Shipments.CreateAsync(OCShipmentMapper.Map(freightPopShipment, buyerID), accessToken: supplierToken);

            await Throttler.RunAsync(freightPopShipment.Items, 100, 1,
            (freightPopShipmentItem) =>
            {
            // currently this creates two items in ordercloud inadvertantely, platform bug is being resolved
                return _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(freightPopShipmentItem, relatedOrder.ID), accessToken: supplierToken);
            });
        }
        private async Task<string> GetBuyerIDForSupplierOrder(MarketplaceOrder relatedOrder)
        {
            var buyerOrderID = relatedOrder.ID.Split("-").First();
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
