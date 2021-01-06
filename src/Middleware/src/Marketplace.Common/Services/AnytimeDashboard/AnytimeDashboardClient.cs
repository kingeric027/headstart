using Flurl;
using Flurl.Http;
using Headstart.Common;
using Headstart.Common.Mappers;
using Headstart.Common.Models;
using Headstart.Common.Services.AnytimeDashboard.Models;
using Headstart.Models;
using Newtonsoft.Json;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.AnytimeDashboard
{
	// The anytime fitness dashboard is a web app with a public, documented API.
	// https://api.anytimefitness.com
	// It can be considered the system of record for Clubs and Staffs
	// Franchise users log into it and then SSO into our Buyer Portal


	// For a sense of scale, as of 11/19/19 Anytime Fiteness has
	// 3618 US gyms withs 8153 distinct staff
	// 325 CA gyms with 826 distinct staff.
	// Note: we are rate limited on this API to 600 requests per minute.  

	public interface IAnytimeDashboardClient
	{
		Task<AFToken> RequestTokenWithAuthCode(string authCode, string redirectUrl);
		Task<List<AFClub>> ListClubsAsync(int page = 1, int pageSize = 100);
		Task<List<AFStaff>> ListStaffForClub(string clubID, int page, int pageSize = 100);
		Task<AFGetStaffResponse> GetASingleStaff(string staffID);
		Task<AFClub> GetClub(string clubID);
		Task<AFCredentials> GetUserDetails(string userBearerToken);
	}

	public class AnytimeDashboardClient : IFranchiseAPI, IAnytimeDashboardClient
	{
		private readonly AppSettings _settings;
		private string _token;

		public string BuyerID { get; }
		public string BrandName { get; }
		public string RedirectUrl { get; }
		public string StorefrontUrl { get; }
		public string StorefrontClientID { get; }

		public AnytimeDashboardClient(AppSettings settings)
		{
			_settings = settings;
			BuyerID = settings.OrderCloudSettings.DataConfig.AfBuyerID;
			BrandName = "Anytime Fitness";
			RedirectUrl = $"{settings.EnvironmentSettings.BaseUrl}/api/anytime/authorize";
			StorefrontClientID = settings.EnvironmentSettings.AFStorefrontClientID;
			StorefrontUrl = settings.EnvironmentSettings.AFStorefrontBaseUrl;
		}

		private async Task<AFToken> RequestTokenWithUserCreds(string username, string password)
		{
			return await RequestToken(new
			{
				grant_type = "password",
				username = username,
				password = password
			});
		}

		public string BuildAuthorizeUrl(string redirectHost, string state)
		{
			return $"{_settings.AnytimeDashboardSettings.AuthUrl}/authorize?" +
				$"response_type=code" +
				$"&client_id={_settings.AnytimeDashboardSettings.ApiToken}" +
				$"&redirect_uri={redirectHost}/api/anytime/authorize" +
				$"&state={state}";
		}

		public async Task<AFToken> RequestTokenWithAuthCode(string authCode, string redirectUrl)
		{
			return await RequestToken(new
			{
				grant_type = "authorization_code",
				client_id = _settings.AnytimeDashboardSettings.ApiToken,
				client_secret = _settings.AnytimeDashboardSettings.ClientSecret,
				redirect_uri = redirectUrl,
				code = authCode
			});
		}

		public async Task<AFGetStaffResponse> GetASingleStaff(string staffID)
		{
			var resource = $"/staffmember/{staffID}?expand=clubs";
			return await GetAsync<AFGetStaffResponse>(resource, null);
		}

		private async Task<AFToken> RequestToken(object formEncodedBody)
		{
			try
			{
				return await $"{_settings.AnytimeDashboardSettings.ApiUrl}/token"
					.WithHeader("SEB-Api-Token", _settings.AnytimeDashboardSettings.ApiToken)
					.WithHeader("Content-Type", "application/x-www-form-urlencoded")
					.PostUrlEncodedAsync(formEncodedBody)
					.ReceiveJson<AFToken>();
			} catch (FlurlHttpException ex)
			{
				throw await BuildException(ex);
			}
		}

		public async Task<AFCredentials> GetUserDetails(string userBearerToken)
		{
			return await GetAsync<AFCredentials>("/credentials", userBearerToken);
		}

		public async Task<AFClub> GetClub(string clubID)
		{
			return await GetAsync<AFClub>($"/clubs/{clubID}", null);
		}

		public async Task<List<AFClub>> ListClubsAsync(int page = 1, int pageSize = 100)
		{
			var resource = $"/clubs?page={page}&pageSize={pageSize}&countryIso2Code=CA|US";
			return await GetAsync<List<AFClub>>(resource, null);
		}

		public async Task<List<AFStaff>> ListStaffForClub(string clubID, int page, int pageSize = 100)
		{
			return await GetAsync<List<AFStaff>>($"/clubs/{clubID}/staff?pageSize={pageSize}&page={page}", null);
		}

		private async Task<T> GetAsync<T>(string resource, string overrideToken)
		{
			var token = overrideToken ?? await GetToken();
			try 
			{ 
				return await $"{_settings.AnytimeDashboardSettings.ApiUrl}{resource}"
					.WithHeader("Authorization", $"Bearer {token}")
					.WithHeader("SEB-Api-Token", _settings.AnytimeDashboardSettings.ApiToken)
					.GetJsonAsync<T>();
			}
			catch (FlurlHttpException ex)
			{
				throw await BuildException(ex);
			}
		}

		private async Task<string> GetToken()
		{
			_token = _token ?? (await RequestTokenWithUserCreds(_settings.AnytimeDashboardSettings.Username, _settings.AnytimeDashboardSettings.Password)).access_token;
			return _token;
		}

		public async Task<IEnumerable<SyncUser>> ListAllUsersOnLocation(SyncLocation location)
		{
			var pageSize = 100;
			var page = 1;
			bool done;
			var totalStaff = new List<SyncUser>();
			do
			{
				var staff = await ListStaffForClub(location.FranchiseeID, page, pageSize);
				var users = staff.Select(s => UserMapper.MapToUser(BuyerID, location, s));
				totalStaff.AddRange(users);
				done = staff.Count < pageSize;
				page++;
			} while (!done);
			return totalStaff;
		}

		public List<string> GetOrderCloudCatalogsIDs(SyncLocation location)
		{
			var countryCatalog = location.Address.Country == "CA" ? _settings.OrderCloudSettings.DataConfig.AfCAOnlyCatalogID : _settings.OrderCloudSettings.DataConfig.AfUSOnlyCatalogID;
			return new List<string> { _settings.OrderCloudSettings.DataConfig.AfAllLocationsCatalogID, countryCatalog };
		}

		public async Task ProcessAllLocations(Func<SyncLocation, Task> proccessLocation)
		{
			var finished = false;
			var page = 1;
			var pageSize = 100;
			do
			{
				var clubs = await ListClubsAsync(page, pageSize);
				var locations = clubs.Select(club => LocationMapper.MapToLocation(BuyerID, club));
				await Throttler.RunAsync(locations, 100, 8, proccessLocation);
				page++;
				finished = clubs.Count() != pageSize;
			} while (!finished);
		}

		public async Task<SSOAuthFields> AuthenticateWithCode(string code)
		{
			var anytimeToken = await RequestTokenWithAuthCode(code, RedirectUrl);
			var credentials = await GetUserDetails(anytimeToken.access_token);
			return UserMapper.MapToAuthFields(BuyerID, credentials);
		}

		private async Task<FranchiseAPIException> BuildException(FlurlHttpException ex)
		{
			var content = ex.Call?.Response?.Content;
			var message = content != null ? await content.ReadAsStringAsync() : ex.Message;
			return new FranchiseAPIException(message, ex);
		}
	}
}
