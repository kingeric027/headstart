using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands.Crud;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Collections.Generic;
using Flurl.Util;

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


		// todo add auth for seller user
		[DocName("GET Product pricing override")]
		[HttpGet, Route("{id}/pricingoverride/buyer/{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<MarketplacePriceSchedule> GetPricingOverride(string id, string buyerID)
		{
			return await _command.GetPricingOverride(id, buyerID, VerifiedUserContext);
		}

		// todo add auth for seller user
		[DocName("CREATE Product pricing override")]
		[HttpPost, Route("{id}/pricingoverride/buyer/{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<MarketplacePriceSchedule> CreatePricingOverride(string id, string buyerID, [FromBody] MarketplacePriceSchedule priceSchedule)
		{
			return await _command.CreatePricingOverride(id, buyerID, priceSchedule, VerifiedUserContext);
		}

		// todo add auth for seller user
		[DocName("PUT Product pricing override")]
		[HttpPut, Route("{id}/pricingoverride/buyer/{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<MarketplacePriceSchedule> UpdatePricingOverride(string id, string buyerID, [FromBody] MarketplacePriceSchedule priceSchedule)
		{
			return await _command.UpdatePricingOverride(id, buyerID, priceSchedule, VerifiedUserContext);
		}

		// todo add auth for seller user
		[DocName("DELETE Product pricing override")]
		[HttpDelete, Route("{id}/pricingoverride/buyer/{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task DeletePricingOverride(string id, string buyerID)
		{
			await _command.DeletePricingOverride(id, buyerID, VerifiedUserContext);
		}

		[DocName("PATCH Product filter option override")]
		[HttpPatch, Route("filteroptionoverride/{id}"), OrderCloudIntegrationsAuth(ApiRole.AdminUserAdmin)]
		public async Task<Product> FilterOptionOverride(string id, [FromBody] Product product)
        {
			IDictionary<string, object> facets = product.xp.Facets;
			var supplierID = product.DefaultSupplierID;
			return await _command.FilterOptionOverride(id, supplierID, facets, VerifiedUserContext);
        }
	}
}
