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

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IShipmentQuery
    {
        Task SyncShipments();
    }

    public class ShipmentQuery : IShipmentQuery
    {
        readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOrderCloudClient _ocFastSupplier;
        public ShipmentQuery(IFreightPopService freightPopService, AppSettings appSettings)
        {
            _freightPopService = freightPopService;
            _ocFastSupplier = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = appSettings.OrderCloudSettings.ApiUrl,
                AuthUrl = appSettings.OrderCloudSettings.AuthUrl,

                // currently just syncing shipments for fast from the four51 freightpop account where they are currently imported
                ClientId = "DA08971F-DD3C-449A-98D2-E39D784761E7",
                ClientSecret = appSettings.OrderCloudSettings.ClientSecret,
                Roles = new[]
                {
                    ApiRole.FullAccess
                }
            });
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

        public async Task SyncShipments()
        {
            try { 
                var orders = await GetOrdersNeedingShipmentAsync(14);

                // abiding by 30 req per minute freightpop limit for now
                var freightPopShipments = await Throttler.RunAsync(orders, 2000, 1,
                    (order) =>
                    {
                        return _freightPopService.GetShipmentsForOrder(order.ID);
                    });

                await Throttler.RunAsync(freightPopShipments, 100, 5, (freightPopShipmentResponse) =>
                {
                    return CreateShipmentsInOrderCloudIfNeeded(freightPopShipmentResponse, orders);
                });
            } catch (Exception ex)
            {
                Console.WriteLine("sadfsds");
            }
        }

        private async Task CreateShipmentsInOrderCloudIfNeeded(Response<List<ShipmentDetails>> freightPopShipmentResponse, List<MarketplaceOrder> orders)
        {
            if (freightPopShipmentResponse.Data == null) return;
            foreach (var freightPopShipment in freightPopShipmentResponse.Data)
            {
                var relatedOrderID = freightPopShipment.Reference1;
                var relatedOrder = orders.Find(order => order.ID == relatedOrderID);
                await CreateShipmentInOrderCloudIfNeeded(freightPopShipment, relatedOrder);
            }
        }

        private async Task CreateShipmentInOrderCloudIfNeeded(ShipmentDetails freightPopShipment, MarketplaceOrder relatedOrder)
        {
            // ordercloud bug prevents listing of shipments for a supplier order when authenticated as a seller user
            var orderCloudShipments = await _ocFastSupplier.Shipments.ListAsync(orderID: relatedOrder.ID);
            if(orderCloudShipments.Meta.TotalCount == 0)
            {
                await AddShipmentToOrderCloud(freightPopShipment, relatedOrder);
            }
        }

        private async Task AddShipmentToOrderCloud(ShipmentDetails freightPopShipment, MarketplaceOrder relatedOrder)
        {
            var ocShipment = await _oc.Shipments.CreateAsync(OCShipmentMapper.Map(freightPopShipment, relatedOrder.FromCompanyID));
            foreach(var freightPopShipmentItem in freightPopShipment.Items)
            {
                await _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(freightPopShipmentItem, relatedOrder.ID));
            }
        }


        public async Task<List<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(int NumberOfDaysBack)
        {
            // gets orders from the present to the number of days back specified in function args
            var twoWeeksAgo = DateTime.UtcNow.AddDays(-NumberOfDaysBack);
            var currentPage = 1;
            var allOrders = new List<MarketplaceOrder>();

            //var firstPage = await GetOrdersNeedingShipmentAsync(currentPage++, twoWeeksAgo);
            //allOrders.AddRange(firstPage.Items);
            var theOrder = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Outgoing, "aKuuWI04J0yy9UJ5LauauQ");
            allOrders.Add(theOrder);

            //var subsequentPages = await Throttler.RunAsync(new string[firstPage.Meta.TotalPages - 1], 100, 5,
            //    (s) =>
            //    {
            //        return GetOrdersNeedingShipmentAsync(currentPage++, twoWeeksAgo);
            //    });

            //foreach (var orderListPage in subsequentPages)
            //{
            //    allOrders.AddRange(orderListPage.Items);
            //}

            return allOrders;
        }

        private Task<ListPage<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(int page, DateTime fromDate)
        {
            return _oc.Orders.ListAsync<MarketplaceOrder>(OrderDirection.Incoming, page: page, pageSize: 100, filters: $"IsSubmitted=true&Status=Open&DateSubmitted>{fromDate}");
        }
    }
}

