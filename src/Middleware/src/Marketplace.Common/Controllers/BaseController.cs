using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Marketplace.Helpers.Models;
using Marketplace.Helpers.Controllers;

namespace Marketplace.Common.Controllers
{
    public class BaseController : MarketplaceController
    {
		public IAppSettings Settings;

		public BaseController(IAppSettings settings)
		{
			Settings = settings;
		}
	}
}
