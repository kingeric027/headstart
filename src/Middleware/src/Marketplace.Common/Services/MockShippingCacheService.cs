using Marketplace.Common.Services.FreightPop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
	// This is called Mock because its doesn't interact with real storage anywhere yet.
	public interface IMockShippingCacheService
	{
		Task<ShippingRate> GetSavedShippingQuote(string orderID, string quoteID);
		Task<IEnumerable<ShippingRate>> SaveShippingQuotes(IEnumerable<ShippingRate> quotes);
	}

	// Where this will be stored? OrderCloud, FreightPoP, BlobStorage
	public class MockShippingCacheService : IMockShippingCacheService
	{
		private readonly IFreightPopService _freightPop;
		public MockShippingCacheService(IFreightPopService freightPop)
		{
			_freightPop = freightPop;
		}

		public async Task<ShippingRate> GetSavedShippingQuote(string orderID, string quoteID)
		{
			// TODO - make sure quote is still valid
			// TODO - Don't use freightPop. Instead, get from cache.
			return (await _freightPop.GetRates(null, null, null)).Data.Rates.First(r => r.Id == quoteID);
		}

		public async Task<IEnumerable<ShippingRate>> SaveShippingQuotes(IEnumerable<ShippingRate> quotes)
		{
			// TODO - implement;
			return quotes;
		}

	}
}
