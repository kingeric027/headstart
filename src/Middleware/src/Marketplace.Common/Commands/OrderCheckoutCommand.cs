using Marketplace.Common.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
	public interface IOrderCheckoutCommand
	{
		Task<MockShippingQuote> GetSingleShippingQuote(string orderID, string shippingQuoteID);
		Task<IEnumerable<MockShippingQuote>> ListShippingQuotes(string orderID);
		Task<Order> SetShippingQuote(string orderID, string shippingQuoteID);
	}

	public class OrderCheckoutCommand : IOrderCheckoutCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IAppSettings _settings;

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

		public OrderCheckoutCommand(IAppSettings settings, IOrderCloudClient oc)
		{
			_oc = oc;
			_settings = settings;
		}

		public async Task<MockShippingQuote> GetSingleShippingQuote(string orderID, string quoteID)
		{
			// TODO - Replace. Get a saved quote from the cache with orderID and quoteID
			return _mockShippingQuoteCache.First(quote => quote.ID == quoteID);
		}

		public async Task<IEnumerable<MockShippingQuote>> ListShippingQuotes(string orderID)
		{
			// TODO - Get all Order Details
			// TODO - Go get fresh shipping quotes from FreightPop based on Order
			return _mockShippingQuoteCache;
		}

		public async Task<Order> SetShippingQuote(string orderID, string quoteID)
		{	
			var shippingQuote = _mockShippingQuoteCache.First(quote => quote.ID == quoteID);
			// Quote not found error.
			var order = await _oc.Orders.GetAsync(OrderDirection.Outgoing, orderID);
			// TODO - check that order is in correct state. e.g., is not already submitted.
			return await _oc.Orders.PatchAsync(OrderDirection.Outgoing, orderID, new PartialOrder()
			{
				ShippingCost = shippingQuote.Cost,
				xp = new
				{
					ShippingQuoteID = shippingQuote.ID     // TODO - This xp is a placeholder until we learn more.
				}
			});
		}
	}
}
