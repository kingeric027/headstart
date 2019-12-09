using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Marketplace.Common.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Marketplace.Common.Queries;
using Marketplace.Common.Services.DevCenter;
using OrderCloud.SDK;

namespace Marketplace.Common.Helpers
{
    public class MarketplaceUserAuthAttribute : AuthorizeAttribute
    {
        /// <param name="roles">Optional list of roles. If provided, user must have just one of them, otherwise authorization fails.</param>
        public MarketplaceUserAuthAttribute(params ApiRole[] roles)
        {
            AuthenticationSchemes = "MarketplaceUser";
            if (roles.Any())
                Roles = string.Join(",", roles);
        }
    }

    public class MarketplaceUserAuthHandler : AuthenticationHandler<MarketplaceUserAuthOptions>
    {
        private readonly IOrderCloudClient _oc;
        private readonly IDevCenterService _dev;

        public MarketplaceUserAuthHandler(IOptionsMonitor<MarketplaceUserAuthOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, IOrderCloudClient oc, IDevCenterService dev)
            : base(options, logger, encoder, clock)
        {
            _oc = oc;
            _dev = dev;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = GetTokenFromAuthHeader();

                if (string.IsNullOrEmpty(token))
                    return AuthenticateResult.Fail("The Marketplace bearer token was not provided in the Authorization header.");

                var jwt = new JwtSecurityToken(token);
                var clientId = jwt.Claims.FirstOrDefault(x => x.Type == "cid")?.Value;
                var usrtype = jwt.Claims.FirstOrDefault(x => x.Type == "usrtype")?.Value;
                if (clientId == null)
                    return AuthenticateResult.Fail("The provided bearer token does not contain a 'cid' (Client ID) claim.");
                
                // we've validated the token as much as we can on this end, go make sure it's ok on OC

                var cid = new ClaimsIdentity("MarketplaceUser");
                cid.AddClaim(new Claim("clientid", clientId));
                cid.AddClaim(new Claim("accesstoken", token));
                
                if (usrtype == "dev")
                {
                    var user = await _dev.GetMe(token);
                    cid.AddClaims(new List<Claim>() { new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(x => x.Type == "role")?.Value) });
                    cid.AddClaim(new Claim("username", jwt.Claims.FirstOrDefault(x => x.Type == "role")?.Value));
                    cid.AddClaim(new Claim("username", user.Username));
                    cid.AddClaim(new Claim("userid", user.ID.ToString()));
                    cid.AddClaim(new Claim("email", user.Email));
                }
                else
                {
                    var user = await _oc.Me.GetAsync(token);
                    if (!user.Active)
                        return AuthenticateResult.Fail("Authentication failure");
                    cid.AddClaim(new Claim("username", user.Username));
                    cid.AddClaim(new Claim("userid", user.ID));
                    cid.AddClaim(new Claim("email", user.Email));
                    cid.AddClaim(new Claim("buyer", user.Buyer?.ID ?? ""));
                    cid.AddClaim(new Claim("supplier", user.Supplier?.ID ?? ""));
                    var t = user.AvailableRoles.Select(r => new Claim(ClaimTypes.Role, r));
                    cid.AddClaims(user.AvailableRoles.Select(r => new Claim(ClaimTypes.Role, r)));
                }

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
