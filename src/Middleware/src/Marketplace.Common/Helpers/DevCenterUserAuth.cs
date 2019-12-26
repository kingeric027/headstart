using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Marketplace.Common.Services.DevCenter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderCloud.SDK;

namespace Marketplace.Common.Helpers
{
    public class DevCenterUserAuthAttribute : AuthorizeAttribute
    {
        /// <param name="roles">Optional list of roles. If provided, user must have just one of them, otherwise authorization fails.</param>
        public DevCenterUserAuthAttribute(params ApiRole[] roles)
        {
            AuthenticationSchemes = "DevCenterUser";
            Roles = $"{string.Join(",", roles.Append(ApiRole.FullAccess).ToArray())},DevCenter";
        }
    }

    public class DevCenterUserAuthHandler : AuthenticationHandler<DevCenterUserAuthOptions>
    {
        private readonly IDevCenterService _dc;

        public DevCenterUserAuthHandler(IOptionsMonitor<DevCenterUserAuthOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, IDevCenterService dc)
            : base(options, logger, encoder, clock)
        {
            _dc = dc;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = GetTokenFromAuthHeader();

                if (string.IsNullOrEmpty(token))
                    return AuthenticateResult.Fail("The DevCenter bearer token was not provided in the Authorization header.");

                var jwt = new JwtSecurityToken(token);
                var clientId = jwt.Claims.FirstOrDefault(x => x.Type == "cid")?.Value;
                var usrtype = jwt.Claims.FirstOrDefault(x => x.Type == "usrtype")?.Value;
                if (clientId == null)
                    return AuthenticateResult.Fail("The provided bearer token does not contain a 'cid' (Client ID) claim.");

                // we've validated the token as much as we can on this end, go make sure it's ok on OC

                var cid = new ClaimsIdentity("DevCenterUser");
                cid.AddClaim(new Claim("clientid", clientId));
                cid.AddClaim(new Claim("accesstoken", token));

                var user = await _dc.GetMe(token);
                cid.AddClaim(new Claim("username", user.Username));
                cid.AddClaim(new Claim("userid", user.ID.ToString()));
                cid.AddClaim(new Claim("email", user.Email));
                cid.AddClaim(new Claim(ClaimTypes.Role, "DevCenter"));

                var ticket = new AuthenticationTicket(new ClaimsPrincipal(cid), "DevCenterUser");
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

    public class DevCenterUserAuthOptions : AuthenticationSchemeOptions
    {

    }
}
