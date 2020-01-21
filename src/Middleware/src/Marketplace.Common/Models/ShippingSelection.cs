using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Common.Models
{
	public class ShippingSelection
	{
		[Required]
		public string SupplierID { get; set; }
		[Required]
		public string ShipFromAddressID { get; set; }
		[Required]
		public string ShippingRateID { get; set; }
	}
}
