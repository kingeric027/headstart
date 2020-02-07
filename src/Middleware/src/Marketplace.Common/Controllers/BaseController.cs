using Marketplace.Models.Controllers;

namespace Marketplace.Common.Controllers
{
    public class BaseController : MarketplaceController
    {
		public AppSettings Settings;

		public BaseController(AppSettings settings)
		{
			Settings = settings;
		}
	}
}
