using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.FreightPop
{
	public class ShippingRate
	{
		public string Id { get; set; }
		public string QuoteId { get; set; } // What is this used for?
		public string AccountName { get; set; }
		public string Carrier { get; set; }
		public string Currency { get; set; }
		public DateTimeOffset DeliveryDate { get; set; }
		public int DeliveryDays { get; set; }
		public string CarrierQuoteId { get; set; }
		public string Service { get; set; }
		//public decimal ListCost { get; set; } Why two costs? Which should we use?
		public decimal TotalCost { get; set; } 
	}
}
