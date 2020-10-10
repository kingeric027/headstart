using Flurl.Http;
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

		private readonly EasyPostConfig _config;
		private const string BaseUrl = "https://api.easypost.com/v2";

		public EasyPostShippingService(EasyPostConfig config)
		{
			_config = config;
		}
		public async Task<ShipEstimateResponse> GetRates(OrderWorksheet order)
		{
			var easyPostShipment = new EasyPostShipment()
			{
				from_address = EasyPostMappers.MapAddress(shipment.ShipFromAddress),
				to_address = EasyPostMappers.MapAddress(shipment.ShipToAddress),
				parcel = EasyPostMappers.MapParcel(shipment.Weight),
				carrier_accounts = carrierAccountIDs.Select(id => new EasyPostCarrierAccount() { id = id }).ToList()
			};


		}

		private async Task<EasyPostShipment> PostShipment(EasyPostShipment shipment)
		{
			return await BaseUrl
				.WithBasicAuth(_config.APIKey, "")
				.AppendPathSegment("shipments")
				.PostJsonAsync(new { shipment })
				.ReceiveJson<EasyPostShipment>();
		}
	}
}
