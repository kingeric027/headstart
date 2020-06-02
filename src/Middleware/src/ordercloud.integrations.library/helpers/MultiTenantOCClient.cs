using System;
using System.IdentityModel.Tokens.Jwt;
using ordercloud.integrations.library.extensions;
using OrderCloud.SDK;

namespace ordercloud.integrations.library
{
    /// <summary>A version of OrderCloudClient that takes all of its config data from an Ordercloud token.
    /// It can be used to support multi-tenancy because it can remove the need to save OC credentials in config settings.
    /// </summary>
    public class MultiTenantOCClient : OrderCloudClient, IOrderCloudClient
    {
		public MultiTenantOCClient(string token) :
			this(new JwtSecurityToken(token)) { }

		public MultiTenantOCClient(JwtSecurityToken jwt) :
			this(jwt.RawPayload, jwt.GetApiUrl(), jwt.GetAuthUrl(), jwt.GetClientID(), jwt.GetExpiresUTC()) { }

		public MultiTenantOCClient(VerifiedUserContext user) :
			this(user.AccessToken, user.ApiUrl, user.AuthUrl, user.ClientID, user.AccessTokenExpiresUTC) { }
	
		private MultiTenantOCClient(string token, string apiUrl, string authUrl, string clientID, DateTime tokenExpiresUTC) : base(
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
