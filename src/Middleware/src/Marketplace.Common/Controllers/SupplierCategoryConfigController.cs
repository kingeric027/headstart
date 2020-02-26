using Cosmonaut;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Helpers.SwaggerTools;
using Marketplace.Models;
using Marketplace.Models.Attributes;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Supplier Category Config\" represents Supplier Category Configuration")]
    [MarketplaceSection.ProductCatalogs(ListOrder = 2)]
    public class SupplierCategoryConfigController : BaseController
    {
        private readonly ISupplierCategoryConfigQuery _query;

        public SupplierCategoryConfigController(AppSettings settings, ISupplierCategoryConfigQuery query) : base(settings)
        {
            _query = query;
        }

        [DocName("GET SupplierCategoryConfig")]
        [HttpGet, Route("marketplace/{marketplaceID}/supplier/category/config"), MarketplaceUserAuth(ApiRole.SupplierReader)]
        public async Task<SupplierCategoryConfig> Get(string marketplaceID)
        {
            return await _query.Get(marketplaceID);
        }
    }
}
