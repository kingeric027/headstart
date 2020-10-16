using Marketplace.Models.Extended;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System.Collections.Generic;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Exceptions;

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
        public List<string> SupplierIDs { get; set; }
        public bool NeedsAttention { get; set; }
        public bool StopShipSync { get; set; }
        public OrderType? OrderType { get; set; }
        public QuoteOrderInfo QuoteOrderInfo { get; set; }
        public ClaimsSummary Returns { get; set; }
        public ClaimsSummary Cancelations { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencySymbol? Currency { get; set; } = null;
        public SubmittedOrderStatus SubmittedOrderStatus { get; set; }
        public string ApprovalNeeded { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public ClaimStatus ClaimStatus { get; set; }
        public string PaymentMethod { get; set; }
        public MarketplaceAddressBuyer ShippingAddress { get; set; }
        public List<ShipMethodSupplierView> SelectedShipMethodsSupplierView { get; set; }
    }

    [SwaggerModel]
    public class ShipMethodSupplierView 
    {
        public int EstimatedTransitDays { get; set; }
        public string Name { get; set; } // e.g. "Fedex PRIORITY_OVERNIGHT"
        public string ShipFromAddressID { get; set; }
        // Do not include buyer's cost. That is none of the supplier's beeswax 
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
    public class ClaimsSummary
    {
        public bool HasClaims { get; set; }
        public bool HasUnresolvedClaims { get; set; }
        public List<ClaimResolutionStatuses> Resolutions { get; set; }
    }

    [SwaggerModel]
    public class ClaimResolutionStatuses
    {
        public string LineItemID { get; set; }
        public string RMANumber { get; set; }
        public bool IsResolved { get; set; }
    }

    public class MarketplaceOrderSubmitPayload
    {
        public MarketplaceOrderSubmitPayloadResponse Response { get; set; }
    }
    
    public class MarketplaceOrderSubmitPayloadResponse
    {
        public MarketplaceOrder Body { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubmittedOrderStatus
    {
        Open,
        Completed,
        Canceled
    }
}