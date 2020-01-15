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
		Task<ShippingRate> GetSavedShippingRateAsync(string orderID, string rateID);
		Task<IEnumerable<ShippingRate>> SaveShippingRatesAsync(IEnumerable<ShippingRate> rates);
	}

	// Where this will be stored? OrderCloud, FreightPoP, BlobStorage
	public class MockShippingCacheService : IMockShippingCacheService
	{
		private readonly IFreightPopService _freightPop;
		public MockShippingCacheService(IFreightPopService freightPop)
		{
			_freightPop = freightPop;
		}

		public async Task<ShippingRate> GetSavedShippingRateAsync(string orderID, string rateID)
		{
			// TODO - make sure rate is still valid
			// TODO - Don't use freightPop. Instead, get from cache.
			return (await _freightPop.GetRatesAsync(null, null, null)).Data.Rates.First(r => r.Id == rateID);
		}

		public async Task<IEnumerable<ShippingRate>> SaveShippingRatesAsync(IEnumerable<ShippingRate> rates)
		{
			// TODO - implement;
			return rates;
		}

	}
}
