using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Helpers;
using Marketplace.Models;
using OrderCloud.SDK;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Microsoft.Extensions.Logging;
using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IShipmentQuery
    {
        Task SyncShipments(ILogger logger);
        Task SyncLatestOrder(ILogger logger);
        Task SyncOneOrder(ILogger logger);
    }

    public class ShipmentQuery : IShipmentQuery
    {
        readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOrderCloudClient _ocFastSupplier;
        private ILogger _logger;
        public ShipmentQuery(IFreightPopService freightPopService, AppSettings appSettings)
        {
            _freightPopService = freightPopService;
           //_ocFastSupplier = new OrderCloudClient(new OrderCloudClientConfig
           //{
           //    ApiUrl = appSettings.OrderCloudSettings.ApiUrl,
           //    AuthUrl = appSettings.OrderCloudSettings.AuthUrl,

           //     // currently just syncing shipments for fast from the four51 freightpop account where they are currently imported
           //     // this oc client is required due to a bug that doesn't allow seller users to access shipments on a supplier order
           //     ClientId = "DA08971F-DD3C-449A-98D2-E39D784761E7",
           //    ClientSecret = appSettings.OrderCloudSettings.ClientSecret,
           //    Roles = new[]
           //    {
           //         ApiRole.FullAccess
           //    }
           //});
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
        }

        public async Task SyncShipments(ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation("Beginning shipment sync for orders submitted in the last last 1 days");
             var orders = await GetOrdersNeedingShipmentAsync(1);
            _logger.LogInformation($"Retrieved {orders.Count()} orders");

            await SyncShipmentsForOrders(orders);
        }

        public async Task SyncLatestOrder(ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation("Beginning shipment sync for latest order");
            var order = await GetLatestOrder();
            _logger.LogInformation($"Retrieved {order.Count()} orders");

            await SyncShipmentsForOrders(order);
      
        }

        public async Task SyncOneOrder(ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation("Beginning shipment sync for orders submitted in the last last 1 days");
            var orders = await GetOneOrder("aKuuWI04J0yy9UJ5LauauQ");
            _logger.LogInformation($"Retrieved {orders.Count()} orders");

            await SyncShipmentsForOrders(orders);
        }

        private async Task SyncShipmentsForOrders(List<MarketplaceOrder> orders)
        {
            try
            {
                var shipmentSyncOrders = await GetShipmentSyncOrders(orders);
                // abiding by 30 req per minute freightpop limit for now
                var shipmentSyncOrdersWithResponses = await Throttler.RunAsync(shipmentSyncOrders, 2000, 1,
                (shipmentSyncOrder) =>
                {
                    return GetShipmentDetailsForShipmentSyncOrder(shipmentSyncOrder);
                });

                var shipmentSyncOrdersToUpdate = shipmentSyncOrdersWithResponses.Where(s => s.FreightPopShipmentResponses!= null && s.FreightPopShipmentResponses.Data != null);

                _logger.LogInformation($"Retrieved {shipmentSyncOrdersToUpdate.Count()} freightpop shipments");

                await Throttler.RunAsync(shipmentSyncOrdersToUpdate, 100, 5, (shipmentSyncOrder) =>
                {
                    return CreateShipmentsInOrderCloudIfNeeded(shipmentSyncOrder);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task<ShipmentSyncOrder> GetShipmentDetailsForShipmentSyncOrder(ShipmentSyncOrder shipmentSyncOrder)
        {
            shipmentSyncOrder.FreightPopShipmentResponses = await _freightPopService.GetShipmentsForOrder(shipmentSyncOrder.FreightPopOrderID);
            return shipmentSyncOrder;

        }

        private async Task<List<ShipmentSyncOrder>> GetShipmentSyncOrders(List<MarketplaceOrder> orders)
        {
            var shipmentSyncOrders = new List<ShipmentSyncOrder>();
            foreach(var order in orders)
            {
                if(order.xp.FreightPopOrderIDs != null)
                {
                    foreach(var freightPopOrder in order.xp.FreightPopOrderIDs)
                    {
                        shipmentSyncOrders.Add(new ShipmentSyncOrder()
                        {
                            OrderCloudOrderID = order.ID,
                            FreightPopOrderID = freightPopOrder,
                            Order = order,
                            FreightPopShipmentResponses = new Response<List<ShipmentDetails>>()
                        });
                    }
                } else
                {
                    // if there are no freightpop order IDs on supplier order, mark it as needing attention
                    _logger.LogWarning($"No FreightPOP orders marked on order {order.ID}. Marked as needing attention");
                    await _oc.Orders.PatchAsync(OrderDirection.Outgoing, order.ID, new PartialOrder() { xp = new { NeedsAttention = true, StopShipSync = true } });
                }
            }
            return shipmentSyncOrders;
        }

        private async Task CreateShipmentsInOrderCloudIfNeeded(ShipmentSyncOrder shipmentSyncOrder)
        {
            foreach (var freightPopShipment in shipmentSyncOrder.FreightPopShipmentResponses.Data)
            {
                await CreateShipmentInOrderCloudIfNeeded(freightPopShipment, shipmentSyncOrder.Order);
            }
        }

        private async Task CreateShipmentInOrderCloudIfNeeded(ShipmentDetails freightPopShipment, MarketplaceOrder relatedOrder)
        {
            try
            {
                // ordercloud bug prevents listing of shipments for a supplier order when authenticated as a seller user
                // oddly another bug is currently changing the tocompanyid of the order to the seller and thus the seller user auth can be used here 
                _logger.LogInformation($"Syncing order number {relatedOrder.ID}");
                var orderCloudShipments = await _oc.Shipments.ListAsync(orderID: relatedOrder.ID);
                var buyerIDForOrder = await GetBuyerIDForSupplierOrder(relatedOrder);

                // needs to be more complex in the future, logic pending discussion around split of orders by supplierid
                if(orderCloudShipments.Meta.TotalCount == 0)
                {
                    await AddShipmentToOrderCloud(freightPopShipment, relatedOrder, buyerIDForOrder);
                } else
                {
                    // if there are already shipments in ordercloud, why was the order not marked as complete
                    _logger.LogWarning($"Preexisting shipment in ordercloud for {relatedOrder.ID}. Marked as needing attention");
                    await _oc.Orders.PatchAsync(OrderDirection.Outgoing, relatedOrder.ID, new PartialOrder() { xp = new { NeedsAttention = true, StopShipSync = true } });
                }
                _logger.LogInformation($"Sync complete for order number {relatedOrder.ID}");
            } catch (Exception ex)       
            {
                _logger.LogError($"Error syncing {relatedOrder.ID}");
                _logger.LogError(ex.Message);
            }
        }

        private async Task AddShipmentToOrderCloud(ShipmentDetails freightPopShipment, MarketplaceOrder relatedOrder, string buyerID)
        {
            var ocShipment = await _oc.Shipments.CreateAsync(OCShipmentMapper.Map(freightPopShipment, buyerID));
            foreach(var freightPopShipmentItem in freightPopShipment.Items)
            {
                // currently this creates two items in ordercloud inadvertantely, platform bug is being resolved
                await _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(freightPopShipmentItem, relatedOrder.ID));
            }
        }

        private async Task<string> GetBuyerIDForSupplierOrder(MarketplaceOrder relatedOrder)
        {
            var buyerOrderID = relatedOrder.ID.Split("-").First();
            var relatedBuyerOrder = await _oc.Orders.GetAsync(OrderDirection.Incoming, buyerOrderID);
            return relatedBuyerOrder.FromCompanyID;
        }

        private async Task<List<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(int NumberOfDaysBack)
        {
            // gets orders from the present to the number of days back specified in function args
            var twoWeeksAgo = DateTime.UtcNow.AddDays(-NumberOfDaysBack);
            var currentPage = 1;
            var allOrders = new List<MarketplaceOrder>();

            var firstPage = await GetOrdersNeedingShipmentAsync(currentPage++, twoWeeksAgo);
            allOrders.AddRange(firstPage.Items);

            var subsequentPages = await Throttler.RunAsync(new string[firstPage.Meta.TotalPages - 1], 100, 5,
                (s) =>
                {
                    return GetOrdersNeedingShipmentAsync(currentPage++, twoWeeksAgo);
                });

            foreach (var orderListPage in subsequentPages)
            {
                allOrders.AddRange(orderListPage.Items);
            }

            return allOrders;
        }

        private async Task<List<MarketplaceOrder>> GetLatestOrder()
        {
            var orders = await _oc.Orders.ListAsync<MarketplaceOrder>(OrderDirection.Outgoing, filters: $"IsSubmitted=true&Status=Open", sortBy: "!DateSubmitted" );
            var firstOrder = orders.Items.First();
            return new List<MarketplaceOrder>()
            {
                firstOrder
            };
        }

        private async Task<List<MarketplaceOrder>> GetOneOrder(string orderID)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Outgoing, orderID);
            return new List<MarketplaceOrder>()
            {
                order
            };
        }

        private Task<ListPage<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(int page, DateTime fromDate)
        {
            return _oc.Orders.ListAsync<MarketplaceOrder>(OrderDirection.Outgoing, page: page, pageSize: 100, filters: $"IsSubmitted=true&Status=Open&DateSubmitted=>{fromDate.ToLongDateString()}&xp.StopShipSync=false");
        }
    }
}

