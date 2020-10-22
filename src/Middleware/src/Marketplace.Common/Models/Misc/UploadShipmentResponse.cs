using System;
using System.Collections.Generic;

namespace Marketplace.Common.Models.Misc
{
    public class Error
    {
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }

    public class Shipment
    {
        public int OrderID { get; set; }
        public int LineItemID { get; set; }
        public int QuantityShipped { get; set; }
        public int? ShipmentID { get; set; }
        public int BuyerID { get; set; }
        public int? Shipper { get; set; }
        public DateTime DateShipped { get; set; }
        public DateTime? DateDelivered { get; set; }
        public int TrackingNumber { get; set; }
        public int? Cost { get; set; }
        public int? FromAddressID { get; set; }
        public int? ToAddressID { get; set; }
        public string Account { get; set; }
        public int? Service { get; set; }
        public string Comments { get; set; }

    }

    public class UploadShipmentResponse
    {
        public List<Error> ErrorList { get; set; }
        public List<Shipment> SuccessfulShipments { get; set; }
    }
}
