using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using System.Collections.Generic;
using ordercloud.integrations.library;
using Marketplace.Models.Models.Marketplace;
using Marketplace.Models.Extended;
using Marketplace.Common.Services.ShippingIntegration.Models;
using ordercloud.integrations.cardconnect;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Orders\" for handling order commands in Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
    [Route("order")]
    public class OrderController : BaseController
    {
        
        private readonly IOrderCommand _command;
        private readonly IOrderSubmitCommand _orderSubmitCommand;
        private readonly ILineItemCommand _lineItemCommand;
        public OrderController(IOrderCommand command, ILineItemCommand lineItemCommand, AppSettings settings, IOrderSubmitCommand orderSubmitCommand) : base(settings)
        {
            _command = command;
            _lineItemCommand = lineItemCommand;
            _orderSubmitCommand = orderSubmitCommand;
        }

        [DocName("Submit Order")]
        [DocComments("Performs validation, submits credit card payment and finally submits order via OrderCloud")]
        [HttpPost, Route("{direction}/{orderID}/submit"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<MarketplaceOrder> Submit(OrderDirection direction, string orderID, [FromBody] OrderCloudIntegrationsCreditCardPayment payment)
        {
            return await _orderSubmitCommand.SubmitOrderAsync(orderID, direction, payment, VerifiedUserContext);
        }

        [DocName("POST Acknowledge Quote Order")]
        [HttpPost, Route("acknowledgequote/{orderID}"), OrderCloudIntegrationsAuth(ApiRole.OrderAdmin)]
        public async Task<Order> AcknowledgeQuoteOrder(string orderID)
        {
            return await _command.AcknowledgeQuoteOrder(orderID);
        }

        [DocName("LIST orders for a specific location as a buyer, ensures user has access to location orders")]
        [HttpGet, Route("location/{locationID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<ListPage<MarketplaceOrder>> ListLocationOrders(string locationID, ListArgs<MarketplaceOrder> listArgs)
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
        public async Task<List<MarketplaceShipmentWithItems>> ListShipmentsWithItems(string orderID)
        {
            return await _command.ListMarketplaceShipmentWithItems(orderID, VerifiedUserContext);
        }

        [DocName("Add or update a line item to an order")]
        [HttpPut, Route("{orderID}/lineitems"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<MarketplaceLineItem> UpsertLineItem(string orderID, [FromBody] MarketplaceLineItem li)
        {
            return await _lineItemCommand.UpsertLineItem(orderID, li, VerifiedUserContext);
        }

        [DocName("Delete a line item")]
        [HttpDelete, Route("{orderID}/lineitems/{lineItemID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task DeleteLineItem(string orderID, string lineItemID)
        {
            await _lineItemCommand.DeleteLineItem(orderID, lineItemID, VerifiedUserContext);
        }

        [DocName("Apply a promotion to an order")]
        [HttpPost, Route("{orderID}/promotions/{promoCode}")]
        public async Task<MarketplaceOrder> AddPromotion(string orderID, string promoCode)
        {
            return await _command.AddPromotion(orderID, promoCode, VerifiedUserContext);
        }

        [DocName("Seller or Supplier Set Line Item Statuses On Order with Related Notification")]
        [HttpPost, Route("{orderID}/{orderDirection}/lineitem/status"), OrderCloudIntegrationsAuth(ApiRole.OrderAdmin)]
        public async Task<List<MarketplaceLineItem>> SellerSupplierUpdateLineItemStatusesWithNotification(string orderID, OrderDirection orderDirection, [FromBody] LineItemStatusChanges lineItemStatusChanges)
        {
            return await _lineItemCommand.UpdateLineItemStatusesAndNotifyIfApplicable(orderDirection, orderID, lineItemStatusChanges, VerifiedUserContext);
        }

        [DocName("Buyer Set Line Item Statuses On Order with Related Notification")]
        [HttpPost, Route("{orderID}/lineitem/status"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<List<MarketplaceLineItem>> BuyerUpdateLineItemStatusesWithNotification(string orderID, [FromBody] LineItemStatusChanges lineItemStatusChanges)
        {
            return await _lineItemCommand.UpdateLineItemStatusesAndNotifyIfApplicable(OrderDirection.Outgoing, orderID, lineItemStatusChanges, VerifiedUserContext);
        }

        [DocName("Apply Automatic Promtions to order and remove promotions no longer valid on order")]
        [HttpPost, Route("{orderID}/applypromotions")]
        public async Task<MarketplaceOrder> ApplyAutomaticPromotions(string orderID)
        {
            return await _command.ApplyAutomaticPromotions(orderID);
        }
    }
}
