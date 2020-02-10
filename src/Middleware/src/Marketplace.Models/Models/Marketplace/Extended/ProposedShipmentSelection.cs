using System.ComponentModel.DataAnnotations;

namespace Marketplace.Models.Extended
{
    public class ProposedShipmentSelection
    {
        [Required]
        public string SupplierID { get; set; }
        [Required]
        public string ShipFromAddressID { get; set; }
        [Required]
        public string ShippingRateID { get; set; }
        public decimal Rate { get; set; }
    }
}
