using Marketplace.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
	// One question is - Do we need to support setting a shipping option at the ShipFrom Address level or the line Item level?
	// This class assumes the answer is ShipFrom Address. In other words, two line Items with the same ShipFrom address share the same shipment. 
	// This is just a best guess for now.
	public class ShippingOptionsFromOneAddress
	{
		public string SupplierID { get; set; }
		public string ShipFromAddressID { get; set; }
		public IEnumerable<MockShippingQuote> Options { get; set; }
	}
}
