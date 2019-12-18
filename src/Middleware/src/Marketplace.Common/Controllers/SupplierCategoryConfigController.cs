using Cosmonaut;
using Marketplace.Common.Models;
using Marketplace.Common.Queries;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
    public class SupplierCategoryConfigController : BaseController
    {
        private ISupplierCategoryConfigQuery _query;

        public SupplierCategoryConfigController(IAppSettings settings, ISupplierCategoryConfigQuery query) : base(settings)
        {
            _query = query;
        }

        [HttpGet, Route("marketplace/{marketplaceID}/supplier/category/config"), MarketplaceUserAuth(ApiRole.SupplierReader)]
        public async Task<SupplierCategoryConfig> Get(string marketplaceID)
        {
            return await _query.Get(marketplaceID);
        }
    }
}
