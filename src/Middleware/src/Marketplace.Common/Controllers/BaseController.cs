using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers
{
	[EnableCors("marketplacecors")]
	[Produces("application/json")]
	public class BaseController : Controller
	{
		public VerifiedUserContext VerifiedUserContext;
		public AppSettings Settings;

		public BaseController(AppSettings settings)
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
