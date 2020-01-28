using System;
using System.Collections.Generic;

namespace Marketplace.Common.Services.FreightPop
{

    public class Order
    {
        public string OrderNumber { get; set; }
        public Carrier Carrier { get; set; }
        public ShipmentItem[] Items { get; set; }
        public PaymentTerm PaymentTerm { get; set; }
        public AccountDetails ThirdPartyAccountInfo { get; set; }
        public DateTime ShipDate { get; set; }
        public OrderAddress ShipperAddress { get; set; }
        public OrderAddress ReturnAddress { get; set; }
        public bool IncludeReturnLabel { get; set; }
        public OrderAddress ConsigneeAddress { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Reference3 { get; set; }
        public string Reference4 { get; set; }
        public string Reference5 { get; set; }
        public string Reference6 { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
        public string ITN { get; set; }
        public AdditionalDetails AdditionalDetails { get; set; }
        // sales order, invoice, etc...
        public string OrderType { get; set; }
        public OrderTransactionType TransactionType { get; set; }
    }
    public class AccountDetails
    {
        public string AccountNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
    public class AdditionalDetails
    {
        public List<Accessorial> Accessorials { get; set; }
        public float CarrierInsuranceAmount { get; set; }
        public CODDetails CODDetails { get; set; }
        public DeliveryConfirmation DeliveryConfirmation { get; set; }
        public string HazmatEmergencyNo { get; set; }
        public string HazmatContact { get; set; }
        public bool IsSaturdayDelivery { get; set; }
        public bool IsDeliveryDutyPaid { get; set; }
        public string SpecialInstructions { get; set; }
    }
    public class CODDetails
{
        public float Amount { get; set; }
        public string ChargeCode { get; set; }
        public CODPaymentType PaymentType { get; set; }
        public OrderAddress RemitAddress { get; set; }
    }
    public class ProductDetail
    {
        public string Country { get; set; }
        public string HTSCode { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public string PackageId { get; set; }
        public float PricePerPiece { get; set; }
        public float PricePerPiece2 { get; set; }
        public float PricePerPiece3 { get; set; }
        public float PricePerPiece4 { get; set; }
        public int Quantity { get; set; }
        public int Quantity2 { get; set; }
        public int Quantity3 { get; set; }
        public int Quantity4 { get; set; }
        public string Unit { get; set; }
        public string OrderNumber { get; set; }
        public string LineType { get; set; }
        public string LineNumber { get; set; }
    }
    public enum DeliveryConfirmation {
        NoSignature,
        AdultSignature,
        Signature
    }

    public enum CODPaymentType
    {
        CustomerCheck,
        CashierCheck,
        CertifiedCheck,
        MoneyOrder
    }
    public enum PaymentTerm
    {
        Recipient,
        Sender,
        ThirdParty
    }
    public enum OrderTransactionType { 
        Sales, 
        Purchasing
    }


}
