using Marketplace.Common.Models;
using Marketplace.Common.Services.FreightPop;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Services
{
	public interface IFreightPopService
	{
		Task<FreightPopResponse<GetRatesData>> GetRates(Address shipFrom, Address shipTo, IEnumerable<LineItem> items);
	}

	public class FreightPopService : IFreightPopService
	{
		public async Task<FreightPopResponse<GetRatesData>> GetRates(Address shipFrom, Address shipTo, IEnumerable<LineItem> items)
		{
			// fake, static data.In the correct format though.
			// TODO - use real endpoint
			return new FreightPopResponse<GetRatesData>
			{
				Code = 200,
				Message = "Success",
				Data = new GetRatesData()
				{
					ErrorMessages = new string[] { },
					Rates = new[] {
						new ShippingRate() {
								Id = "12345",
								QuoteId = "sample string 6",
								CarrierQuoteId = "sample string 7",
								AccountName = "sample string 2",
								DeliveryDays = 1,
								DeliveryDate = DateTime.Parse("2020-01-03T08:03:18.0552937-08:00"),
								ListCost = 9.0M,
								TotalCost = 10.0M,
								Currency = "USD",
								Carrier = "Fedex",
								Service = "Priority Air"
						},
						new ShippingRate()
						{
								Id = "34567",
								QuoteId = "sample string 6",
								CarrierQuoteId = "sample string 7",
								AccountName = "sample string 2",
								DeliveryDays = 2,
								DeliveryDate = DateTime.Parse("2020-01-03T08:03:18.0552937-08:00"),
								ListCost = 5.0M,
								TotalCost = 6.0M,
								Currency = "USD",
								Carrier = "UPS",
								Service = "Air Elite"
						},
						new ShippingRate()
						{
								Id = "56789",
								QuoteId = "sample string 6",
								CarrierQuoteId = "sample string 7",
								AccountName = "sample string 2",
								DeliveryDays = 3,
								DeliveryDate = DateTime.Parse("2020-01-03T08:03:18.0552937-08:00"),
								ListCost = 2.0M,
								TotalCost = 4.0M,
								Currency = "USD",
								Carrier = "Unites State Postal Service",
								Service = "Ground"
						},
					}
				}
			};
		}
	}
}

