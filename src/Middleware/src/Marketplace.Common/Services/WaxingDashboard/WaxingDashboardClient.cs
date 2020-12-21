using Flurl;
using Flurl.Http;
using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Common.Services.WaxingDashboard.Models;
using Marketplace.Models;
using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Marketplace.Common.Services.WazingDashboard
{
	// The WTC API was custom built for Four51 and details about it were first shared 8/21/2020.
	// The PDF in this folder has most of the details. 
	// The main contact for this API is Gianluca Balzano lucab@sebrands.com
	public interface IWaxDashboardClient
	{
		Task<WTCToken> RequestTokenWithAuthCode(string authCode, string redirectUrl);
		Task<WTCList<WTCStudio>> ListStudiosAsync();
		Task<WTCResponse<WTCStudio>> GetStudioAsync(string studioID);
		Task<List<WTCStaff>> ListStudioStaffAsync(string studioID, int page, int pageSize = 100);
		Task<WTCResponse<WTCStaff>> GetStaff(string staffID);
	}

	public class WaxDashboardClient: IFranchiseAPI, IWaxDashboardClient
	{
		private readonly AppSettings _settings;
		private readonly string[] WTCEastRegionStates = { "AL", "CT", "DE", "FL", "GA", "IN", "KY", "ME", "MD", "MA", "MI", "MS", "NH", "NJ", "NY", "NC", "OH", "PA", "RI", "SC", "TN", "VT", "VA", "WV" };

		private string _token;
		public string BuyerID { get; }
		public string BrandName { get; }
		public string RedirectUrl { get; }
		public string StorefrontUrl { get; }
		public string StorefrontClientID { get; }

		public WaxDashboardClient(AppSettings settings)
		{
			_settings = settings;
			BuyerID = settings.OrderCloudSettings.DataConfig.WtcBuyerID;
			BrandName = "Waxing The City";
			RedirectUrl = $"{settings.EnvironmentSettings.BaseUrl}/api/waxing/authorize";
			StorefrontClientID = settings.EnvironmentSettings.WTCStorefrontClientID;
			StorefrontUrl = settings.EnvironmentSettings.WTCStorefrontBaseUrl;
		}

		public string BuildAuthorizeUrl(string redirectHost, string state)
		{
			var scope = "https://identity.sebrands.com/scopes/brand.waxingthecity%20https://identity.sebrands.com/scopes/staff.readonly";
			return $"{_settings.WaxDashboardSettings.AuthUrl}/connect/authorize?scope={scope}".SetQueryParams(new
			{
				response_type = "code",
				client_id = _settings.WaxDashboardSettings.UserClientID,
				redirect_uri = $"{redirectHost}/api/waxing/authorize",
				code_challenge_method = "S256",
				state = state,
				code_challenge = Coding.GenerateCodeChallange(_settings.WaxDashboardSettings.CodeVerifier)
			});
		}

		public async Task<WTCToken> RequestTokenWithAuthCode(string authCode, string redirectUrl)
		{
			try
			{
				return await $"{_settings.WaxDashboardSettings.AuthUrl}/connect/token"
					.PostUrlEncodedAsync(new
					{
						grant_type = "authorization_code",
						client_id = _settings.WaxDashboardSettings.UserClientID,
						client_secret = _settings.WaxDashboardSettings.UserClientSecret,
						redirect_uri = redirectUrl,
						code_verifier = _settings.WaxDashboardSettings.CodeVerifier,
						code = authCode
					}).ReceiveJson<WTCToken>();
			}catch (Exception e)
			{
				throw e;
			}
		}

		private async Task<WTCToken> RequestTokenWithClientCreds()
		{
			return await $"{_settings.WaxDashboardSettings.AuthUrl}/connect/token"
				.WithHeader("Content-Type", "application/x-www-form-urlencoded")
				.PostUrlEncodedAsync(new
				{
					grant_type = "client_credentials",
					client_id = _settings.WaxDashboardSettings.M2MClientID,
					client_secret = _settings.WaxDashboardSettings.M2MClientSecret
				})
				.ReceiveJson<WTCToken>();
		}

		public async Task<WTCList<WTCStudio>> ListStudiosAsync()
		{
			return await GetAsync<WTCList<WTCStudio>>("/locations");
		}

		public async Task<WTCResponse<WTCStudio>> GetStudioAsync(string studioID)
		{
			return await GetAsync<WTCResponse<WTCStudio>>($"/locations/{studioID}");
		}

		public async Task<List<WTCStaff>> ListStudioStaffAsync(string studioID, int page, int pageSize = 100)
		{
			var staff = await GetAsync<WTCList<WTCStaff>>($"/locations/{studioID}/staff?pageNumber={page}&pageSize={pageSize}");
			return staff.items;
		}

		public async Task<WTCResponse<WTCStaff>> GetStaff(string staffID)
		{
			return await GetAsync<WTCResponse<WTCStaff>>($"/staff/{staffID}");
		}

		private async Task<T> GetAsync<T>(string resource)
		{
			var token = await GetToken();
			return await $"{_settings.WaxDashboardSettings.ApiUrl}{resource}"
				.WithHeader("Authorization", $"Bearer {token}")
				.WithHeader("ClientID", _settings.WaxDashboardSettings.M2MClientID)
				.GetJsonAsync<T>();
		}

		private async Task<string> GetToken()
		{
			_token = _token ?? (await RequestTokenWithClientCreds()).access_token;
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
				var staff = await ListStudioStaffAsync(location.FranchiseeID, page, pageSize);
				var users = staff.Select(s => UserMapper.MapToUser(BuyerID, s));
				totalStaff.AddRange(users);
				done = staff.Count < pageSize;
				page++;
			} while (!done);
			return totalStaff;
		}
		public List<string> GetOrderCloudCatalogsIDs(SyncLocation location)
		{
			var regionCatalog = WTCEastRegionStates.Contains(location.Address.State) ? _settings.OrderCloudSettings.DataConfig.WtcEastCatalogID : _settings.OrderCloudSettings.DataConfig.WtcWestCatalogID;
			return new List<string> { _settings.OrderCloudSettings.DataConfig.WtcAllLocationsCatalogID, regionCatalog };
		}

		public async Task<SSOAuthFields> AuthenticateWithCode(string code)
		{
			var waxingToken = await RequestTokenWithAuthCode(code, RedirectUrl);
			var jwt = new JwtSecurityTokenHandler().ReadJwtToken(waxingToken.access_token);
			var staff = (await GetStaff(jwt.Subject)).items.FirstOrDefault();
			return UserMapper.MapToAuthFields(BuyerID, staff);
		}
		public async Task ProcessAllLocations(Func<SyncLocation, Task> proccessLocation)
		{
			var studios = await ListStudiosAsync();
			var locations = studios.items.Select(s => LocationMapper.MapToLocation(BuyerID, s));
			await Throttler.RunAsync(locations, 100, 8, proccessLocation);
		}
	}
}
