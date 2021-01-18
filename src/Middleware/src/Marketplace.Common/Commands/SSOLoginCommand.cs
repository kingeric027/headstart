using System;
using System.Threading.Tasks;
using Headstart.Common;
using Headstart.Common.Mappers;
using Headstart.Common.Models;
using OrderCloud.SDK;

namespace Headstart.Common.Commands
{
    public interface ISSOLoginCommand
	{
		string BuildAuthorizeUrl(FranchiseEnum franchise, string state);
		Task<string> BuildStorefrontUrl(FranchiseEnum franchise, string code, string state);
	}

	public class SSOLoginCommand : ISSOLoginCommand
	{
		private readonly AppSettings _settings;
		private readonly IOrderCloudClient _oc;

		public SSOLoginCommand(AppSettings settings, IOrderCloudClient oc)
		{
			_settings = settings;
			_oc = oc;
		}

		public string BuildAuthorizeUrl(FranchiseEnum franchise, string state)
		{
			var iFranchise = franchise.GetIFranchise(_settings);
			return iFranchise.BuildAuthorizeUrl(_settings.EnvironmentSettings.BaseUrl, state);
		}


		public async Task<string> BuildStorefrontUrl(FranchiseEnum franchise, string code, string state)
		{
			if (code == null)
			{
				throw new Exception("Authorization request must contain a 'code' query parameter");
			}
			var stateObj = Coding.DecodeState(state);
			var franchiseInterface = franchise.GetIFranchise(_settings);
			SSOAuthFields auth;
			try
			{
				auth = await franchiseInterface.AuthenticateWithCode(code);
			}
			catch
			{
				throw new Exception($"Error authorizing to the {franchiseInterface.BrandName} API with code: {code}");
			}
			var buyerID = franchiseInterface.BuyerID;
			var user = await GetOrdercloudUser(buyerID, auth.ID, auth.Username);
			var token = await GetOrdercloudToken(buyerID, franchiseInterface.StorefrontClientID, user.ID);
			return $"{franchiseInterface.StorefrontUrl}{stateObj.Path}?ssoToken={token.access_token}";
		}

		private async Task<User> GetOrdercloudUser(string buyerID, string userID, string userName)
		{
			try
			{
				return await _oc.Users.GetAsync(buyerID, userID);
			}
			catch 
			{
				throw new Exception($"Ordercloud user with ID {userID} and username {userName} was not found");
			}
		}

		private async Task<AccessToken> GetOrdercloudToken(string buyerID, string clientID, string userID) {
			return await _oc.Users.GetAccessTokenAsync(buyerID, userID, new ImpersonateTokenRequest()
			{
				ClientID = clientID,
				Roles = new[] {
					ApiRole.MeAddressAdmin,
					ApiRole.AddressAdmin,
					ApiRole.MeAdmin,
					ApiRole.MeCreditCardAdmin,
					ApiRole.MeXpAdmin,
					ApiRole.UserGroupAdmin,
					ApiRole.ApprovalRuleAdmin,
					ApiRole.Shopper,
					ApiRole.BuyerUserAdmin,
					ApiRole.BuyerReader,
					ApiRole.PasswordReset,
					ApiRole.SupplierReader,
					ApiRole.SupplierAddressReader
				},
				CustomRoles = new []
				{
					"MPApprovalRuleAdmin",
					"MPLocationOrderApprover",
					"MPLocationViewAllOrders",
					"MPLocationCreditCardAdmin",
					"MPLocationPermissionAdmin",
					"MPLocationResaleCertAdmin",
					"MPLocationNeedsApproval",
					"MPLocationAddressAdmin",
					"DocumentReader"
				}
			});
		}
	}
}
