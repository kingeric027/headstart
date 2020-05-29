using System.Collections.Generic;
using Marketplace.Models.Extended;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
	public class MarketplaceOrder : Order<OrderXp, MarketplaceUser, MarketplaceAddressBuyer>
    {
	}

    [SwaggerModel]
    public class OrderXp
    {
        public string AvalaraTaxTransactionCode { get; set; }
        public List<string> ShipFromAddressIDs { get; set; }
        public List<string> SupplierIDs { get; set;}
        public bool NeedsAttention { get; set; }
        public bool StopShipSync { get; set; }
        public OrderType? OrderType { get; set; }
        public QuoteOrderInfo QuoteOrderInfo { get; set; }
        public OrderReturnInfo OrderReturnInfo { get; set; }
        public string Currency { get; set; }
    }

    [SwaggerModel]
    public class OrderDetails
    {
        public MarketplaceOrder Order { get; set; }
        public ListPage<LineItem> LineItems { get; set; } 
        public ListPage<OrderPromotion> Promotions { get; set; } 
        public ListPage<Payment> Payments { get; set; } 
        public ListPage<OrderApproval> Approvals { get; set; } 
    }

	[SwaggerModel]
    public class OrderReturnInfo
    {
        public bool HasReturn { get; set; }
        public string RMANumber { get; set; }
        public bool Resolved { get; set; }
    }
    
    public class MarketplaceOrderSubmitPayload
    {
        public MarketplaceOrderSubmitPayloadResponse Response { get; set; }
    }
    
    public class MarketplaceOrderSubmitPayloadResponse
    {
        public MarketplaceOrder Body { get; set; }
    }
}