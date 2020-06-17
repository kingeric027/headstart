using System.Threading.Tasks;
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

		private readonly IMarketplaceProductCommand _productCommand;
		public MeController(AppSettings settings, IMarketplaceProductCommand productCommand) : base(settings)
		{
			_productCommand = productCommand;
		}

		[DocName("GET Super Product")]
		[HttpGet, Route("products/{productID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<SuperMarketplaceProduct> GetSuperProduct(string productID)
		{
			return await _productCommand.MeGet(productID, VerifiedUserContext);
		}
	}
}
