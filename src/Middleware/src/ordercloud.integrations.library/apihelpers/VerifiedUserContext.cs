﻿using ordercloud.integrations.library.extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ordercloud.integrations.library
{
    public class VerifiedUserContext
    {
        public ClaimsPrincipal Principal { get; }
        private readonly JwtSecurityToken _token;

        public VerifiedUserContext(ClaimsPrincipal principal)
        {
            Principal = principal;
            if (Principal.Claims.Any())
                _token = new JwtSecurityTokenHandler().ReadJwtToken(this.AccessToken);
        }

        public string UsrType
        {
            get { return _token.GetClaim("usrtype"); }
		}

        public string UserID
        {
            get { return Principal.Claims.First(c => c.Type == "userid").Value; }
        }
        public string Username
        {
            get { return Principal.Claims.First(c => c.Type == "username").Value; }
        }
        public string ClientID
        {
            get { return Principal.Claims.First(c => c.Type == "clientid").Value; }
        }
        public string Email
        {
            get { return Principal.Claims.First(c => c.Type == "email").Value; }
        }
        public string SupplierID
        {
            get { return Principal.Claims.First(c => c.Type == "supplier").Value; }
        }
        public string BuyerID
        {
            get { return Principal.Claims.First(c => c.Type == "buyer").Value; }
        }

        public string AccessToken
        {
            get { return Principal.Claims.First(c => c.Type == "accesstoken").Value; }
        }

        public string AuthUrl
        {
            get { return _token.GetClaim("iss"); }
        }

        public string ApiUrl
        {
			get { return _token.GetClaim("aud"); }
		}

        public DateTime AccessTokenExpiresUTC
        {
            get
            {
                return _token.GetClaim("exp").UnixToDateTime();
            }
        }

		public DateTime NotValidBeforeUTC
		{
			get
			{
				return _token.GetClaim("nbf").UnixToDateTime();
			}
		}
	}
}
