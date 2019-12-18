using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Models
{
	// This class is called "Mock" because it is provisional until we determine the real model from FreightPop.
	public class MockShippingQuote
	{
		public string ID { get; set; }
		public int DeliveryDays { get; set; }
		public decimal Cost { get; set; }
		public string Service { get; set; } // e.g. Ground, Air, Priority, ect.
		public string Carrier { get; set; } // e.g. Fedex, UPS, USPS, ect.
	}
}
