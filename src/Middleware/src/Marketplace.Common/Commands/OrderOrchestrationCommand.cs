using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using System.Linq;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Helpers;
using Marketplace.Common.Services.FreightPop.Models;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;

namespace Marketplace.Common.Commands
{
    public interface IOrderOrchestrationCommand
    {
        Task<List<MarketplaceSupplier>> GetSuppliersNeedingSync();
        Task<List<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(string supplierToken);
        Task<List<ShipmentSyncOrder>> GetShipmentSyncOrders(List<MarketplaceOrder> orders);
        Task<List<ShipmentSyncOrder>> GetShipmentDetailsForShipmentSyncOrders(List<ShipmentSyncOrder> shipmentSyncOrders);
        Task CreateShipmentsInOrderCloudIfNeeded(ShipmentSyncOrder shipmentSyncOrder, string supplierToken);
        string SupplierID();
    }

    public class OrderOrchestrationCommand : IOrderOrchestrationCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly string _supplierID;
        private IFreightPopService _freightPopService;
        private readonly AppSettings _appSettings;

        public OrderOrchestrationCommand(IOrderCloudClient oc, IFreightPopService freightPopService, AppSettings appSettings, string supplierID)
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
        }


        public string SupplierID()
        {
            return _supplierID;
        }

        public async Task<List<MarketplaceSupplier>> GetSuppliersNeedingSync()
        {
            // todo support over 100 suppliers
            var suppliers = await _oc.Suppliers.ListAsync<MarketplaceSupplier>(pageSize: 100, filters: $"xp.SyncFreightPop=true");
            return suppliers.Items.ToList();
        }

        public async Task<List<MarketplaceOrder>> GetOrdersNeedingShipmentAsync(string supplierToken)
        {
            var currentPage = 1;
            var allOrders = new List<MarketplaceOrder>();

            var firstPage = await GetOrdersNeedingShipmentAsync(currentPage++, supplierToken);
            allOrders.AddRange(firstPage.Items);

            if (firstPage.Meta.TotalPages > 0)
            {
                var subsequentPages = await Throttler.RunAsync(new string[firstPage.Meta.TotalPages - 1], 100, 5,
                    (s) =>
                    {
                        return GetOrdersNeedingShipmentAsync(currentPage++, supplierToken);
                    });

                foreach (var orderListPage in subsequentPages)
                {
                    allOrders.AddRange(orderListPage.Items);
                }
            }

            return allOrders;
        }

        public async Task<List<ShipmentSyncOrder>> GetShipmentSyncOrders(List<MarketplaceOrder> orders)
        {
            var shipmentSyncOrders = new List<ShipmentSyncOrder>();
            foreach (var order in orders)
            {
                if (order.xp.ShipFromAddressIDs != null)
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
                else
                {
                    // if there are no freightpop order IDs on supplier order, mark it as needing attention
                    await _oc.Orders.PatchAsync(OrderDirection.Outgoing, order.ID, new PartialOrder() { xp = new { NeedsAttention = true, StopShipSync = true } });
                }
            }
            return shipmentSyncOrders;
        }

        public async Task<List<ShipmentSyncOrder>> GetShipmentDetailsForShipmentSyncOrders(List<ShipmentSyncOrder> shipmentSyncOrders)
        {
            var shipmentSyncOrdersWithResponses = await Throttler.RunAsync(shipmentSyncOrders, 2000, 1,
             (shipmentSyncOrder) =>
             {
                 return GetShipmentDetailsForShipmentSyncOrder(shipmentSyncOrder);
             });
            return shipmentSyncOrdersWithResponses.ToList();
        }

        public async Task<ShipmentSyncOrder> GetShipmentDetailsForShipmentSyncOrder(ShipmentSyncOrder shipmentSyncOrder)
        {
            shipmentSyncOrder.FreightPopShipmentResponses = await _freightPopService.GetShipmentsForOrder(shipmentSyncOrder.FreightPopOrderID);
            return shipmentSyncOrder;

        }

        public async Task CreateShipmentsInOrderCloudIfNeeded(ShipmentSyncOrder shipmentSyncOrder, string supplierToken)
        {
            foreach (var shipmentDetails in shipmentSyncOrder.FreightPopShipmentResponses.Data)
            {
                await CreateShipmentInOrderCloudIfNeeded(shipmentDetails, shipmentSyncOrder.Order, supplierToken);
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
            foreach (var freightPopShipmentItem in freightPopShipment.Items)
            {
                // currently this creates two items in ordercloud inadvertantely, platform bug is being resolved
                await _oc.Shipments.SaveItemAsync(ocShipment.ID, OCShipmentItemMapper.Map(freightPopShipmentItem, relatedOrder.ID), accessToken: supplierToken);
            }
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
