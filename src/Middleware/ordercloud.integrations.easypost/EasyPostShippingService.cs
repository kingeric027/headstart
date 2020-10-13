using Flurl.Http;
using Microsoft.Extensions.Azure;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace ordercloud.integrations.easypost
{
	public class AddressPair : IEquatable<AddressPair>
	{
		public Address ShipFrom { get; set; }
		public Address ShipTo { get; set; }

		public bool Equals(AddressPair other)
		{
			return (ShipFrom.Street1 == other.ShipFrom.Street1) &&
					(ShipFrom.Zip == other.ShipFrom.Zip) &&
					(ShipFrom.City == other.ShipFrom.City) &&
					(ShipTo.Street1 == other.ShipTo.Street1) &&
					(ShipTo.Zip == other.ShipTo.Zip) &&
					(ShipTo.City == other.ShipTo.City);
		}

		public override int GetHashCode()
		{
			return 1; // force Equals to be called for comparison
		}
	}

	public interface IEasyPostShippingService
	{
		Task<ShipEstimateResponse> GetRates(IEnumerable<IGrouping<AddressPair, LineItem>> groupedLineItems, IEnumerable<string> carrierAccountIDs);
	}


	public class EasyPostShippingService : IEasyPostShippingService
	{
		private readonly EasyPostConfig _config;
		private const string BaseUrl = "https://api.easypost.com/v2";

		public EasyPostShippingService(EasyPostConfig config)
		{
			_config = config;
		}

		public async Task<ShipEstimateResponse> GetRates(IEnumerable<IGrouping<AddressPair, LineItem>> groupedLineItems, IEnumerable<string> carrierAccountIDs)
		{
			var easyPostShipments = groupedLineItems.Select(li => EasyPostMappers.MapShipment(li, carrierAccountIDs));
			var easyPostResponse = await Task.WhenAll(easyPostShipments.Select(PostShipment));
			var shipEstimateResponse = new ShipEstimateResponse
			{
				ShipEstimates = groupedLineItems.Select((lineItems, index) =>
				{
					var shipMethods = EasyPostMappers.MapRates(easyPostResponse[index].rates);
					return new ShipEstimate()
					{
						ID = easyPostResponse[index]?.id,
						ShipMethods = shipMethods, // This will get filtered down based on carrierAccounts
						ShipEstimateItems = lineItems.Select(li => new ShipEstimateItem() { LineItemID = li.ID, Quantity = li.Quantity }).ToList(),
						xp = { AllShipMethods = shipMethods } // This is being saved so we have all data to compare rates across carrierAccounts
					};
				}).ToList(),
			};
			return shipEstimateResponse;
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
