using System.Threading.Tasks;
using Marketplace.CMS.Models;
using Marketplace.CMS.Queries;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Controllers.CMS;
using Marketplace.Helpers;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
	[DocComments("Me and my stuff")]
	[Route("me")]
	public class MeController : BaseController
	{

		private readonly IMarketplaceProductCommand _productCommand;
		public MeController(AppSettings settings, IMarketplaceProductCommand productCommand) : base(settings)
		{
			_productCommand = productCommand;
		}

		[DocName("GET Super Product")]
		[HttpGet, Route("products/{id}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<SuperMarketplaceProduct> Get(string id)
		{
			return await _productCommand.MeGet(id, VerifiedUserContext);
		}
	}
}
