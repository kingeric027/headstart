using System.Threading.Tasks;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
	[DocComments("Me and my stuff")]
	[MarketplaceSection.Marketplace(ListOrder = 10)]
	[Route("me")]
	public class MeController : BaseController
	{

		private readonly IMeProductCommand _meProductCommand;
		private readonly IMarketplaceProductCommand _productCommand;
		public MeController(AppSettings settings, IMarketplaceProductCommand productCommand, IMeProductCommand meProductCommand) : base(settings)
		{
			_productCommand = productCommand;
			_meProductCommand = meProductCommand;
		}

		[DocName("GET Super Product")]
		[HttpGet, Route("products/{productID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<SuperMarketplaceMeProduct> GetSuperProduct(string productID)
		{
			return await _meProductCommand.Get(productID, VerifiedUserContext);
		}

		[DocName("LIST products")]
		[HttpGet, Route("products"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<ListPageWithFacets<MarketplaceMeProduct>> ListMeProducts(ListArgs<MarketplaceMeProduct> args)
		{
			return await _meProductCommand.List(args, VerifiedUserContext);
		}
	}
}
