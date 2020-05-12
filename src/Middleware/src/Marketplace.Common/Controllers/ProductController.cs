using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;
using Marketplace.Common.Commands.Crud;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
	[DocComments("\"Products\" represents Products for Marketplace")]
	[MarketplaceSection.Marketplace(ListOrder = 3)]
	[Route("products")]
	public class ProductController : BaseController
	{

		private readonly IMarketplaceProductCommand _command;
		public ProductController(AppSettings settings, IMarketplaceProductCommand command) : base(settings)
		{
			_command = command;
		}

		[DocName("GET Super Product")]
		[HttpGet, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
		public async Task<SuperMarketplaceProduct> Get(string id)
		{
			return await _command.Get(id, VerifiedUserContext);
		}

		[DocName("LIST Super Product")]
		[HttpGet, OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
		public async Task<ListPage<SuperMarketplaceProduct>> List(ListArgs<MarketplaceProduct> args)
		{
			return await _command.List(args, VerifiedUserContext);
		}

		[DocName("POST Super Product")]
		[HttpPost, OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<SuperMarketplaceProduct> Post([FromBody] SuperMarketplaceProduct obj)
		{
			return await _command.Post(obj, this.VerifiedUserContext);
		}

		[DocName("PUT Super Product")]
		[HttpPut, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<SuperMarketplaceProduct> Put([FromBody] SuperMarketplaceProduct obj, string id)
		{
			return await _command.Put(id, obj, this.VerifiedUserContext);
		}

		[DocName("DELETE Product")]
		[HttpDelete, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task Delete(string id)
		{
			await _command.Delete(id, VerifiedUserContext);
		}
	}
}
