using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orchestration.Common.Queries;
using OrderCloud.SDK;

namespace Orchestration.Common.Helpers
{
    public class OrchestrationUserAuthAttribute : AuthorizeAttribute
    {
        /// <param name="roles">Optional list of roles. If provided, user must have just one of them, otherwise authorization fails.</param>
        public OrchestrationUserAuthAttribute(params Models.ApiRole[] roles)
        {
            AuthenticationSchemes = "OrchestrationUser";
            if (roles.Any())
                Roles = string.Join(",", roles);
        }
    }

    public class OrchestrationUserAuthHandler : AuthenticationHandler<OrchestrationUserAuthOptions>
    {
        private readonly IOrderCloudClient _oc;
        private readonly SupplierQuery _supplier;

        public OrchestrationUserAuthHandler(IOptionsMonitor<OrchestrationUserAuthOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, IOrderCloudClient oc, SupplierQuery supplier)
            : base(options, logger, encoder, clock)
        {
            _oc = oc;
            _supplier = supplier;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = GetTokenFromAuthHeader();

                if (string.IsNullOrEmpty(token))
                    return AuthenticateResult.Fail("The Orchestration bearer token was not provided in the Authorization header.");

                var jwt = new JwtSecurityToken(token);
                var clientId = jwt.Claims.FirstOrDefault(x => x.Type == "cid")?.Value;
                if (clientId == null)
                    return AuthenticateResult.Fail("The provided bearer token does not contain a 'cid' (Client ID) claim.");
                
                // we've validated the token as much as we can on this end, go make sure it's ok on OC
                var user = await _oc.Me.GetAsync(token);
                if (!user.Active)
                    return AuthenticateResult.Fail("Authentication failure");

                var supplier = await _supplier.GetByUserID(user.ID);
                if (supplier == null)
                    return AuthenticateResult.Fail("Authentication failure. Stored credentials not found");

                var cid = new ClaimsIdentity("OrchestrationUser");
                cid.AddClaim(new Claim("accesstoken", token));
                cid.AddClaim(new Claim("username", user.Username));
                cid.AddClaim(new Claim("userid", user.ID));
                cid.AddClaim(new Claim("email", user.Email));
                cid.AddClaim(new Claim("buyer", user.Buyer?.ID ?? ""));
                cid.AddClaim(new Claim("clientid", supplier.ClientID));
                cid.AddClaim(new Claim("supplier", supplier.id));
                cid.AddClaim(new Claim("secret", supplier.ClientSecret));
                cid.AddClaims(user.AvailableRoles.Select(r => new Claim(ClaimTypes.Role, r)));

                var ticket = new AuthenticationTicket(new ClaimsPrincipal(cid), "OrchestrationUser");
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

    public class OrchestrationUserAuthOptions : AuthenticationSchemeOptions
    {

    }
}
