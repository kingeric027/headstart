using Flurl.Http;
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
		Task<Response<GetRatesData>> GetRatesAsync(RateRequestBody rateRequestBody);
	}

	public class FreightPopService : IFreightPopService
	{
		private readonly AppSettings _appSettings;
		private readonly IFlurlClient _flurl;
		private string accessToken;
		public FreightPopService(AppSettings appSettings, IFlurlClient flurl)
		{
			_flurl = flurl;
			_appSettings = appSettings;
			AuthenticateAync();
		}
		
		private IFlurlRequest Get(string resource)
		{
			return _flurl.Request($"{_appSettings.FreightPopSettings.BaseUrl}/{resource}")
				.WithHeader("Authorization", $"Bearer {accessToken}");
		}
		private IFlurlRequest Post(string resource)
		{
			return _flurl.Request($"{_appSettings.FreightPopSettings.BaseUrl}/{resource}")
				.WithHeader("Authorization", $"Bearer {accessToken}");
		}
		private async Task AuthenticateAync()
		{
			var passwordGrantRequest = new PasswordGrantRequestData
			{
				Username = _appSettings.FreightPopSettings.Username,
				Password = _appSettings.FreightPopSettings.Password
			};
			var passwordGrantResponse = await _flurl.Request($"{_appSettings.FreightPopSettings.BaseUrl}/token/getToken").PostJsonAsync(passwordGrantRequest).ReceiveJson<Response<PasswordGrantResponseData>>();
			accessToken = passwordGrantResponse.Data.AccessToken;
		}
		public async Task<Response<GetRatesData>> GetRatesAsync(RateRequestBody rateRequestBody)
		{
			// troubleshoot how to convert the type back to the generic with freightPOP Response type
			
			var rateRequestResponse = await Get("rate/getRates").PostJsonAsync(rateRequestBody).ReceiveJson<Response<GetRatesData>>();
			return rateRequestResponse;
		}
			//public async Task<FreightPopResponse<GetRatesData>> GetRatesAsync(Address shipFrom, Address shipTo, IEnumerable<LineItem> items)
			//{
			//	// fake, static data.In the correct format though.
			//	// TODO - use real endpoint
			//	return new FreightPopResponse<GetRatesData>
			//	{
			//		Code = 200,
			//		Message = "Success",
			//		Data = new GetRatesData()
			//		{
			//			ErrorMessages = new string[] { },
			//			Rates = new[] {
			//				new ShippingRate() {
			//						Id = "12345",
			//						CarrierQuoteId = "sample string 7",
			//						AccountName = "sample string 2",
			//						DeliveryDays = 1,
			//						DeliveryDate = DateTime.Parse("2020-01-03T08:03:18.0552937-08:00"),
			//						TotalCost = 10.0M,
			//						Currency = "USD",
			//						Carrier = "Fedex",
			//						Service = "Priority Air"
			//				},
			//				new ShippingRate()
			//				{
			//						Id = "34567",
			//						CarrierQuoteId = "sample string 7",
			//						AccountName = "sample string 2",
			//						DeliveryDays = 2,
			//						DeliveryDate = DateTime.Parse("2020-01-03T08:03:18.0552937-08:00"),
			//						TotalCost = 6.0M,
			//						Currency = "USD",
			//						Carrier = "UPS",
			//						Service = "Air Elite"
			//				},
			//				new ShippingRate()
			//				{
			//						Id = "56789",
			//						CarrierQuoteId = "sample string 7",
			//						AccountName = "sample string 2",
			//						DeliveryDays = 3,
			//						DeliveryDate = DateTime.Parse("2020-01-03T08:03:18.0552937-08:00"),
			//						TotalCost = 4.0M,
			//						Currency = "USD",
			//						Carrier = "Unites State Postal Service",
			//						Service = "Ground"
			//				},
			//			}
			//		}
			//	};
			//}
		}
	}

