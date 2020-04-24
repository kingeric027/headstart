using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Marketplace.Helpers.Models
{
    public class VerifiedUserContext
    {
        private readonly ClaimsPrincipal _c;
        private readonly JwtSecurityToken _token;

        public VerifiedUserContext(ClaimsPrincipal c)
        {
            _c = c;
            if (_c.Claims.Any())
                _token = new JwtSecurityTokenHandler().ReadJwtToken(this.AccessToken);
        }

        public string UsrType
        {
            get { return _token.Payload.FirstOrDefault(t => t.Key == "usrtype").Value?.ToString(); }
        }

        public string UserID
        {
            get { return _c.Claims.First(c => c.Type == "userid").Value; }
        }
        public string Username
        {
            get { return _c.Claims.First(c => c.Type == "username").Value; }
        }
        public string ClientID
        {
            get { return _c.Claims.First(c => c.Type == "clientid").Value; }
        }
        public string Email
        {
            get { return _c.Claims.First(c => c.Type == "email").Value; }
        }
        public string SupplierID
        {
            get { return _c.Claims.First(c => c.Type == "supplier").Value; }
        }
        public string BuyerID
        {
            get { return _c.Claims.First(c => c.Type == "buyer").Value; }
        }

		public string AuthUrl
		{
			get { return _c.Claims.First(c => c.Type == "iss").Value; }
		}

		public string ApiUrl
		{
			get { return _c.Claims.First(c => c.Type == "aud").Value; }
		}

		public string AccessToken
        {
            get { return _c.Claims.First(c => c.Type == "accesstoken").Value; }
        }
    }
}
