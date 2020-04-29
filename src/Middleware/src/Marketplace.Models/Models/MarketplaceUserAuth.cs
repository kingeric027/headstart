using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Extensions;
using Marketplace.Helpers.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderCloud.SDK;

namespace Marketplace.Models
{
	public class MarketplaceUserAuthAttribute : AuthorizeAttribute, IApiAuthAttribute
	{
        public ApiRole[]  ApiRoles { get; }
		/// <param name="roles">Optional list of roles. If provided, user must have just one of them, otherwise authorization fails.</param>
		public MarketplaceUserAuthAttribute(params ApiRole[] roles)
		{
			AuthenticationSchemes = "MarketplaceUser";
			ApiRoles = roles.Append(ApiRole.FullAccess).ToArray(); // Full Access is always included in the list of roles that give access to a resource.  
			Roles = string.Join(",", ApiRoles);
        }
	}

	public class MarketplaceUserAuthHandler : AuthenticationHandler<MarketplaceUserAuthOptions>
    {
        public MarketplaceUserAuthHandler(IOptionsMonitor<MarketplaceUserAuthOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) {}

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = GetTokenFromAuthHeader();

                if (string.IsNullOrEmpty(token))
                    return AuthenticateResult.Fail("The Marketplace bearer token was not provided in the Authorization header.");

                var jwt = new JwtSecurityToken(token);
                var clientId = jwt.Claims.FirstOrDefault(x => x.Type == "cid")?.Value;
				var apiUrl = jwt.Claims.FirstOrDefault(x => x.Type == "aud")?.Value;
				var authUrl = jwt.Claims.FirstOrDefault(x => x.Type == "iss")?.Value;
				var tokenExpiresUTC = jwt.Claims.FirstOrDefault(t => t.Type == "exp").Value.ToString().UnixToDateTime();

				var usrtype = jwt.Claims.FirstOrDefault(x => x.Type == "usrtype")?.Value;
                if (clientId == null)
                    return AuthenticateResult.Fail("The provided bearer token does not contain a 'cid' (Client ID) claim.");
                
                // we've validated the token as much as we can on this end, go make sure it's ok on OC

                var cid = new ClaimsIdentity("MarketplaceUser");
                cid.AddClaim(new Claim("clientid", clientId));
                cid.AddClaim(new Claim("accesstoken", token));
                
                var user = await new MyOrderCloudClient(token, apiUrl, authUrl, clientId, tokenExpiresUTC).Me.GetAsync(token);
                if (!user.Active)
                    return AuthenticateResult.Fail("Authentication failure");
                cid.AddClaim(new Claim("username", user.Username));
                cid.AddClaim(new Claim("userid", user.ID));
                cid.AddClaim(new Claim("email", user.Email ?? ""));
                cid.AddClaim(new Claim("buyer", user.Buyer?.ID ?? ""));
                cid.AddClaim(new Claim("supplier", user.Supplier?.ID ?? ""));
                cid.AddClaims(user.AvailableRoles.Select(r => new Claim(ClaimTypes.Role, r)));

                var ticket = new AuthenticationTicket(new ClaimsPrincipal(cid), "MarketplaceUser");
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private string GetTokenFromAuthHeader()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var header))
                return null;

            var parts = header.FirstOrDefault()?.Split(new[] { ' ' }, 2);
            if (parts?.Length != 2)
                return null;

            return parts[0] != "Bearer" ? null : parts[1].Trim();
        }
    }

    public class MarketplaceUserAuthOptions : AuthenticationSchemeOptions
    {

    }
}
