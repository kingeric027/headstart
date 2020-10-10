using OrderCloud.SDK;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace ordercloud.integrations.easypost
{
	public interface IEasyPostShippingService
	{
		Task<ShipEstimateResponse> GetRates(OrderWorksheet order);
	}


	public class EasyPostShippingService : IEasyPostShippingService
	{
		public async Task<ShipEstimateResponse> GetRates(OrderWorksheet order)
		{
			return new ShipEstimateResponse();
		}
	}
}
