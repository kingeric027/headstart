using System.Collections.Generic;
using Marketplace.Models.Extended;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.exchangerates;
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
		[JsonConverter(typeof(StringEnumConverter))]
		public CurrencySymbol Currency { get; set; }
    }

	[JsonConverter(typeof(StringEnumConverter))]
	public enum OrderType
	{
		Standard,
		Quote
	}

	[SwaggerModel]
    public class OrderDetails
    {
        public MarketplaceOrder Order { get; set; }
        public IList<LineItem> LineItems { get; set; } 
        public IList<OrderPromotion> Promotions { get; set; } 
        public IList<Payment> Payments { get; set; } 
        public IList<OrderApproval> Approvals { get; set; } 
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