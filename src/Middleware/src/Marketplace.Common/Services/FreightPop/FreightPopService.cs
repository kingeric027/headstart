using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Marketplace.Common.Services.FreightPop.Models;

namespace Marketplace.Common.Services.FreightPop
{
	public interface IFreightPopService
	{
		Task<Response<dynamic>> ImportOrderAsync(List<OrderRequest> orderRequestBody);
		Task<Response<GetRatesData>> GetRatesAsync(RateRequestBody rateRequestBody);
		Task<Response<List<ShipmentDetails>>> GetShipmentsForOrder(string orderID);
		Task<Response<List<ShipmentDetails>>> GetShipmentsByDate(int daysAgo);
	}

	public class FreightPopService : IFreightPopService
	{
		private readonly string _username;
		private readonly string _password;
		private readonly string _freightPopBaseUrl;
		private readonly IFlurlClient _flurl;
		private string accessToken;
		private DateTime tokenExpireDate;
		public FreightPopService(AppSettings appSettings)
		{
			_flurl = new FlurlClient();
			_username = appSettings.FreightPopSettings.Username;
			_password = appSettings.FreightPopSettings.Password;
			_freightPopBaseUrl = appSettings.FreightPopSettings.BaseUrl;
		}

		private IFlurlRequest MakeRequest(string resource)
		{
			return _flurl.Request($"{_freightPopBaseUrl}/{resource}")
				.WithHeader("Authorization", $"Bearer {accessToken}");
		}
		private async Task AuthenticateAync()
		{
			// authenticate if more than 13 days from the previous token request
			if(DateTime.Now > tokenExpireDate)
			{
				var passwordGrantRequest = new PasswordGrantRequestData
				{
					Username = _username,
					Password = _password
				};
				var passwordGrantResponse = await _flurl.Request($"{_freightPopBaseUrl}/token/getToken").PostJsonAsync(passwordGrantRequest).ReceiveJson<Response<PasswordGrantResponseData>>();
			
				// freightpop tokens expire in 14 days but I don't know how to decode (not JWTs) so I am maintaining the expire date when the toke is grabbed
				tokenExpireDate = DateTime.Now.AddDays(13);
				accessToken = passwordGrantResponse.Data.AccessToken;
			}
		}
		public async Task<Response<GetRatesData>> GetRatesAsync(RateRequestBody rateRequestBody)
		{

			// change back when freightpop test is back up
			try
			{
				await AuthenticateAync();
				var rateRequestResponse = await MakeRequest("rate/getRates").PostJsonAsync(rateRequestBody).ReceiveJson<Response<GetRatesData>>();
				return rateRequestResponse;
			} catch (Exception ex) { 
				var mockRatesResponse = new Response<GetRatesData>()
				{
					Code = 200,
					Data = new GetRatesData()
					{
						Rates = new List<ShippingRate>()
						{
							new ShippingRate()
							{
								AccountName = "mock rate account",
								Currency = "USD",
								ListCost = 10,
								Carrier = "mock carrier",
								CarrierQuoteId = "mock1",
								DeliveryDays = 3,
								QuoteId = "mockratequote1",
								TotalCost = 10,
								Id = "rate1",
								Service = "Ground",
							},
							new ShippingRate()
							{
								AccountName = "mock rate account",
								Currency = "USD",
								ListCost = 20,
								Carrier = "mock carrier",
								CarrierQuoteId = "mock1",
								DeliveryDays = 2,
								QuoteId = "mockratequote2",
								TotalCost = 20,
								Id = "rate2",
								Service = "Express",
							},
							new ShippingRate()
							{
								AccountName = "mock rate account",
								Currency = "USD",
								ListCost = 30,
								Carrier = "mock carrier",
								CarrierQuoteId = "mock1",
								DeliveryDays = 1,
								QuoteId = "mockratequote3",
								TotalCost = 30,
								Id = "rate3",
								Service = "Air",
							},

						}
					},
					Message = "Mock Rate Response"
				};
				return mockRatesResponse;
			}
		}

		public async Task<Response<dynamic>> ImportOrderAsync(List<OrderRequest> orderRequestBody)
		{
			await AuthenticateAync();
			var orderRequestResponse = await MakeRequest("order/ImportOrder").PostJsonAsync(orderRequestBody).ReceiveJson<Response<dynamic>>();
			return orderRequestResponse;
		}
		public async Task<Response<List<ShipmentDetails>>> GetShipmentsForOrder(string orderID)
		{
			await AuthenticateAync();
			var getShipmentResponse = await MakeRequest($"shipment/getShipment?id={orderID}&type={GetShipmentBy.OrderNo}").GetAsync().ReceiveJson<Response<List<ShipmentDetails>>>();
			return getShipmentResponse;
		}
		public async Task<Response<List<ShipmentDetails>>> GetShipmentsByDate(int daysAgo)
		{
			await AuthenticateAync();
			var getShipmentResponse = await MakeRequest($"shipment/getShipment?id={GetDateStringForQuery(daysAgo)}&type={GetShipmentBy.Date}").GetAsync().ReceiveJson<Response<List<ShipmentDetails>>>();
			return getShipmentResponse;
		}
		private string GetDateStringForQuery(int daysAgo)
		{
			var dateToQuery = DateTime.Now.AddDays(-daysAgo);
			return dateToQuery.ToString("MM/dd/yyyy");
		}
	}
}

