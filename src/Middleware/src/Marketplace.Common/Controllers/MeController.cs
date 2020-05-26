using System.Threading.Tasks;
using Marketplace.Common.Commands.Crud;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;
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
		[HttpGet, Route("products/{id}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<SuperMarketplaceProduct> Get(string id)
		{
			return await _productCommand.MeGet(id, VerifiedUserContext);
		}
	}
}
