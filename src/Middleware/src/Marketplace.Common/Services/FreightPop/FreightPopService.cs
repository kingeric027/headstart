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
			var rateRequestResponse = await MakeRequest("rate/getRates").PostJsonAsync(rateRequestBody).ReceiveJson<Response<GetRatesData>>();
			return rateRequestResponse;
		}
	}
}

