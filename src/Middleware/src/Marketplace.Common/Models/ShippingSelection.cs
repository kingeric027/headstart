using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
	public class ShippingSelection
	{
		public string SupplierID { get; set; }
		public string ShipFromAddressID { get; set; }
		public string ShippingQuoteID { get; set; }
	}
}
