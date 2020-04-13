using System.Collections.Generic;
using Marketplace.Helpers.Attributes;
using Marketplace.Models.Extended;
using OrderCloud.SDK;

namespace Marketplace.Models
{
    [SwaggerModel]
	public class MarketplaceOrder : Order<OrderXp, UserXp, BuyerAddressXP>
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