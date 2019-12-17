using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Marketplace.Helpers.Models;

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
}
