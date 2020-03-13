using System.Collections.Generic;
using Marketplace.Models.Extended;
using OrderCloud.SDK;

namespace Marketplace.Models
{
	public class MarketplaceOrder : Order<OrderXp, UserXp, BuyerAddressXP>
    {
        // will be on the ordercloud model when integration events are deployed to OC prod
        public string CalculateEventsUpToDate { get; set; } 
        public string ShippingRatesEventUpToDate { get; set; } 
	}

    public class OrderXp
    {
        public string AvalaraTaxTransactionCode { get; set; }
        public int NumberOfSupplierOrders { get; set; }
        public bool NeedsAttention { get; set; }
        public bool StopShipSync { get; set; }

        // these need to be reworked, quote order info is required currently on every order
        //public OrderType? OrderType { get; set; }
        //public QuoteOrderInfo QuoteOrderInfo { get; set; }
    }
}