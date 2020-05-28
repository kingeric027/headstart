using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using System.Collections.Generic;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Orders\" for handling order commands in Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("order")]
    public class OrderController : BaseController
    {
        
        private readonly IOrderCommand _command;
        public OrderController(IOrderCommand command, AppSettings settings) : base(settings)
        {
            _command = command;
        }

        [DocName("POST Acknowledge Quote Order")]
        // todo update auth
        [HttpPost]
        [Route("acknowledgequote/{orderID}")]
        public async Task<Order> AcknowledgeQuoteOrder(string orderID)
        {
            return await _command.AcknowledgeQuoteOrder(orderID);
        }

        [DocName("LIST orders for a specific location as a buyer, ensures user has access to location orders")]
        [HttpGet, Route("location/{locationID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<ListPage<Order>> ListLocationOrders(string locationID, ListArgs<MarketplaceOrder> listArgs)
        {
            return await _command.ListOrdersForLocation(locationID, listArgs, VerifiedUserContext);
        }

        [DocName("GET order details as buyer, ensures user has access to location orders or created the order themselves")]
        [HttpGet, Route("{orderID}/details"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<OrderDetails> GetOrderDetails(string orderID)
        {
            return await _command.GetOrderDetails(orderID, VerifiedUserContext);
        }

        [DocName("GET order shipments as buyer, ensures user has access to location orders or created the order themselves")]
        [HttpGet, Route("{orderID}/shipmentswithitems"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<List<MarketplaceShipmentWithItems>> GetOrderShipmentsWithItems(string orderID)
        {
            return await _command.GetMarketplaceShipmentWithItems(orderID, VerifiedUserContext);
        }
    }
}
