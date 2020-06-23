using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;
using Marketplace.Common.Commands.Crud;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Catalogs\" for product groupings and visibility in Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 2)]
	[Route("buyers")]
	public class CatalogController : BaseController
    {
        
        private readonly IMarketplaceCatalogCommand _command;
        public CatalogController(IMarketplaceCatalogCommand command, AppSettings settings) : base(settings)
        {
            _command = command;
        }

		[DocName("GET Catalog")]
		[HttpGet, Route("{buyerID}/catalogs/{catalogID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
		public async Task<MarketplaceCatalog> Get(string buyerID, string catalogID)
		{
			return await _command.Get(buyerID, catalogID, VerifiedUserContext);
		}

		[DocName("LIST Catalogs")]
		[HttpGet, Route("{buyerID}/catalogs"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin, ApiRole.ProductReader)]
		public async Task<ListPage<MarketplaceCatalog>> List(ListArgs<MarketplaceCatalog> args, string buyerID)
		{
			return await _command.List(buyerID, args, VerifiedUserContext);
		}

		[DocName("POST Catalog")]
		[HttpPost, Route("buyers/{buyerID}/catalogs"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<MarketplaceCatalog> Post([FromBody] MarketplaceCatalog obj, string buyerID)
		{
			return await _command.Post(buyerID, obj, VerifiedUserContext);
		}

		[DocName("PUT Catalog")]
		[HttpPut, Route("{buyerID}/catalogs/{catalogID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<MarketplaceCatalog> Put([FromBody] MarketplaceCatalog obj, string buyerID, string catalogID)
		{
			return await _command.Put(buyerID, catalogID, obj, this.VerifiedUserContext);
		}

		[DocName("DELETE Catalog")]
		[HttpDelete, Route("{buyerID}/catalogs/{catalogID}"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task Delete(string buyerID, string catalogID)
		{
			await _command.Delete(buyerID, catalogID, VerifiedUserContext);
		}
	}
}
