using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Helpers.Helpers
{
	public class MyOrderCloudClient : OrderCloudClient, IOrderCloudClient
	{
		public MyOrderCloudClient(VerifiedUserContext user) : base(
			new OrderCloudClientConfig()
			{
				ApiUrl = user.ApiUrl,
				AuthUrl = user.AuthUrl,
				ClientId = user.ClientID,
				Roles = new[] { ApiRole.FullAccess }
			}
		) {
			TokenResponse = new TokenResponse()
			{
				AccessToken = user.AccessToken,
				ExpiresUtc = user.AccessTokenExpiresUTC
			};
		}

		public MyOrderCloudClient(string token, string apiUrl, string authUrl, string clientID, DateTime tokenExpiresUTC) : base(
			new OrderCloudClientConfig()
			{
				ApiUrl = apiUrl,
				AuthUrl = authUrl,
				ClientId = clientID,
				Roles = new[] { ApiRole.FullAccess }
			}
		)
		{  
			TokenResponse = new TokenResponse()
			{
				AccessToken = token,
				ExpiresUtc = tokenExpiresUTC
			};
		}
	}
}
