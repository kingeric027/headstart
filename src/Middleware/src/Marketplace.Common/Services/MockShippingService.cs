using Marketplace.Common.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
	public interface IMockShippingService
	{
		Task<MockShippingQuote> GetSavedShipmentQuote(string orderID, string shippingQuoteID);
		Task<IEnumerable<MockShippingQuote>> GenerateShipmentQuotes(IEnumerable<LineItem> lineItems);
	}

	public class MockShippingService
	{
		// TODO - Remove. Where this will be stored? OrderCloud, FreightPoP, BlobStorage
		private readonly IEnumerable<MockShippingQuote> _mockShippingQuoteCache = new[] {
			new MockShippingQuote() {
					ID = "12345",
					DeliveryDays = 1,
					Cost = 18.99M,
					Carrier = "Fedex",
					Service = "Priority Air"
			},
			new MockShippingQuote()
			{
					ID = "34567",
					DeliveryDays = 1,
					Cost = 15.99M,
					Carrier = "UPS",
					Service = "Air Elite"
			},
			new MockShippingQuote()
			{
					ID = "56789",
					DeliveryDays = 2,
					Cost = 10.99M,
					Carrier = "Unites State Postal Service",
					Service = "Ground"
			},
		};

		public async Task<MockShippingQuote> GetSavedShipmentQuote(string orderID, string quoteID)
		{
			// TODO - Replace. Get a saved quote from the cache with orderID and quoteID
			return _mockShippingQuoteCache.First(quote => quote.ID == quoteID);
		}

		public async Task<IEnumerable<MockShippingQuote>> GenerateShipmentQuotes(IEnumerable<LineItem> shipment)
		{
			// TODO - Go get fresh shipping quotes from FreightPop.
			return _mockShippingQuoteCache;
		}
	}

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
