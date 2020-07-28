using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Orchestration\" represents objects exposed for orchestration control")]
    [MarketplaceSection.Orchestration(ListOrder = 1)]
    [Route("orchestration/{clientId}")]
    public class OrchestrationProductController : BaseController
    {
        private readonly IOrchestrationCommand _command;

        public OrchestrationProductController(AppSettings settings, IOrchestrationCommand command) : base(settings)
        {
            _command = command;
        }

        [DocName("POST SuperProduct")]
        [HttpPost, Route("hydrated"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<SuperMarketplaceProduct> PostHydratedProduct([FromBody] SuperMarketplaceProduct obj,
            string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Catalog")]
        [HttpPost, Route("catalog"), OrderCloudIntegrationsAuth(ApiRole.CatalogAdmin)]
        public async Task<MarketplaceCatalog> PostCatalog([FromBody] MarketplaceCatalog obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Product")]
        [HttpPost, Route("product"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceProduct> PostProduct([FromBody] MarketplaceProduct obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Product Facet")]
        [HttpPost, Route("productfacet"), OrderCloudIntegrationsAuth(ApiRole.ProductFacetAdmin)]
        public async Task<MarketplaceProductFacet> PostProductFacet([FromBody] MarketplaceProductFacet obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Price Schedule")]
        [HttpPost, Route("priceschedule"), OrderCloudIntegrationsAuth(ApiRole.PriceScheduleAdmin)]
        public async Task<MarketplacePriceSchedule> PostPriceSchedule([FromBody] MarketplacePriceSchedule obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Product Assignment")]
        [HttpPost, Route("productassignment"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceProductAssignment> PostProductAssignment([FromBody] MarketplaceProductAssignment obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Spec")]
        [HttpPost, Route("spec"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceSpec> PostSpec([FromBody] MarketplaceSpec obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Spec Option")]
        [HttpPost, Route("specoption"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceSpecOption> PostSpecOption([FromBody] MarketplaceSpecOption obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Spec Product Assignment")]
        [HttpPost, Route("specproductassignment"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<MarketplaceSpecProductAssignment> PostSpecProductAssignment([FromBody] MarketplaceSpecProductAssignment obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }

        [DocName("POST Catalog Product Assignment")]
        [HttpPost, Route("catalogproductassignment"), OrderCloudIntegrationsAuth(ApiRole.CatalogAdmin)]
        public async Task<MarketplaceCatalogAssignment> PostCatalogProductAssignment([FromBody] MarketplaceCatalogAssignment obj, string clientId)
        {
            return await _command.SaveToQueue(obj, this.VerifiedUserContext, this.VerifiedUserContext.SupplierID, clientId);
        }
    }
}
