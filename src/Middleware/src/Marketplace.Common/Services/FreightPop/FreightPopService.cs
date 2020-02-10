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
		}

		private IFlurlRequest MakeRequest(string resource)
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
			// temporarily here to prevent auth issues with transient
			await AuthenticateAync();
			var rateRequestResponse = await MakeRequest("rate/getRates").PostJsonAsync(rateRequestBody).ReceiveJson<Response<GetRatesData>>();
			return rateRequestResponse;
		}

		public async Task<Response<dynamic>> ImportOrderAsync(List<OrderRequest> orderRequestBody)
		{
			// temporarily here to prevent auth issues with transient
			await AuthenticateAync();
			var orderRequestResponse = await MakeRequest("order/ImportOrder").PostJsonAsync(orderRequestBody).ReceiveJson<Response<dynamic>>();
			return orderRequestResponse;
		}
	}
}

