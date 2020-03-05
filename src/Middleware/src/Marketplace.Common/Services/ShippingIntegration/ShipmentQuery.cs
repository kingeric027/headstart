using System;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.ShippingIntegration
{
    public interface IShipmentQuery
    {
        Task GetShipments();
    }

    public class ShipmentQuery : IShipmentQuery
    {
        readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        public ShipmentQuery(IFreightPopService freightPopService, AppSettings appSettings)
        {
            _freightPopService = freightPopService;
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

        public async Task GetShipments()
        {
            // temporary test to see if durable function is getting deployed and running properly
            var order = _oc.Orders.GetAsync(OrderDirection.Incoming, "5zDsgPJw00O9LJtWndH92w");
            var dateTimeString = DateTime.Now.ToString();
            await _oc.Orders.PatchAsync(OrderDirection.Incoming, "5zDsgPJw00O9LJtWndH92w", new PartialOrder
            {
                xp = new
                {
                    AvalaraTaxTransactionCode = $"test-{dateTimeString}"
                }
            });
        }
    }
}

