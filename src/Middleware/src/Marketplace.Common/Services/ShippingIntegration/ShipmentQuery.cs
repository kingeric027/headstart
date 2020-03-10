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

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IShipmentQuery
    {
        Task SyncShipments(ILogger logger);
        Task SyncLatestOrder(ILogger logger);
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

        private async Task SyncShipmentsForOrders(List<MarketplaceOrder> orders)
        {
            try
            {
                // abiding by 30 req per minute freightpop limit for now
                var freightPopShipmentsResponses = await Throttler.RunAsync(orders, 2000, 1,
                (order) =>
                {
                    return _freightPopService.GetShipmentsForOrder(order.ID);
                });

                var freightPopShipments = freightPopShipmentsResponses.Where(s => s != null && s.Data != null);

                _logger.LogInformation($"Retrieved {freightPopShipments.Count()} freightpop shipments");

                await Throttler.RunAsync(freightPopShipments, 100, 5, (freightPopShipmentResponse) =>
                {
                    return CreateShipmentsInOrderCloudIfNeeded(freightPopShipmentResponse, orders);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task CreateShipmentsInOrderCloudIfNeeded(Response<List<ShipmentDetails>> freightPopShipmentResponse, List<MarketplaceOrder> orders)
        {
            foreach (var freightPopShipment in freightPopShipmentResponse.Data)
            {
                var relatedOrderID = freightPopShipment.Reference1;
                var relatedOrder = orders.Find(order => order.ID == relatedOrderID);
                await CreateShipmentInOrderCloudIfNeeded(freightPopShipment, relatedOrder);
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
                await _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(freightPopShipmentItem, relatedOrder.ID));
            }
        }

        private async Task<string> GetBuyerIDForSupplierOrder(MarketplaceOrder relatedOrder)
        {
            if(relatedOrder.xp == null || relatedOrder.xp.RelatedBuyerOrder == null)
            {
                throw new Exception("RelatedBuyerOrder is not marked on this order");
            }
            var buyerOrderID = relatedOrder.xp.RelatedBuyerOrder;
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

        private Task<ListPage<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(int page, DateTime fromDate)
        {
            return _oc.Orders.ListAsync<MarketplaceOrder>(OrderDirection.Outgoing, page: page, pageSize: 100, filters: $"IsSubmitted=true&Status=Open&DateSubmitted>{fromDate}");
        }
    }
}

