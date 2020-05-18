using Marketplace.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using Marketplace.Models.Extended;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Supplier Category Config\" represents Supplier Category Configuration")]
    [MarketplaceSection.Marketplace(ListOrder = 5)]
    public class SupplierCategoryConfigController : BaseController
    {
        private readonly ISupplierCategoryConfigQuery _query;

        public SupplierCategoryConfigController(AppSettings settings, ISupplierCategoryConfigQuery query) : base(settings)
        {
            _query = query;
        }

        [DocName("GET SupplierCategoryConfig")]
        [HttpGet, Route("marketplace/{marketplaceID}/supplier/category/config"), OrderCloudIntegrationsAuth(ApiRole.SupplierReader)]
        public async Task<SupplierCategoryConfig> Get(string marketplaceID)
        {
            return await _query.Get(marketplaceID);
        }
    }
}
