using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderCloud.SDK;

namespace Marketplace.Models.Controllers
{
    [EnableCors("marketplacecors")]
    [Produces("application/json")]
    public class MarketplaceController : Controller
    {
        public VerifiedUserContext VerifiedUserContext;

        public MarketplaceController()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            VerifiedUserContext = new VerifiedUserContext(User);
            base.OnActionExecuting(context);
        }
    }
}
