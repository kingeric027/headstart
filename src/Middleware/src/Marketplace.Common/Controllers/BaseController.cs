using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marketplace.Common.Controllers
{
    [EnableCors("marketplacecors")]
    [Produces("application/json")]
    public class BaseController : Controller
    {
        public VerifiedUserContext VerifiedUserContext;
        public IAppSettings Settings;

        public BaseController(IAppSettings settings)
        {
            Settings = settings;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            VerifiedUserContext = new VerifiedUserContext(User);
            base.OnActionExecuting(context);
        }
    }

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

        public string AccessToken
        {
            get { return _c.Claims.First(c => c.Type == "accesstoken").Value; }
        }
    }
}
