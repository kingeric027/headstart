using Marketplace.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using Marketplace.Models.Extended;
using ordercloud.integrations.library;
using ordercloud.integrations.cms;
using Marketplace.Models;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Supplier Filter Config\" represents Supplier Category Configuration")]
    [MarketplaceSection.Marketplace(ListOrder = 5)]
    public class SupplierFilterConfigController : BaseController
    {
        private readonly IDocumentQuery _query;

        public SupplierFilterConfigController(AppSettings settings, IDocumentQuery query) : base(settings)
        {
            _query = query;
        }

        [DocName("GET SupplierCategoryConfig")]
        [HttpGet, Route("/supplierfilterconfig"), OrderCloudIntegrationsAuth(ApiRole.Shopper, ApiRole.SupplierReader)]
        public async Task<ListPage<SupplierFilterConfigDocument>> Get()
        {
            var config = await _query.List<SupplierFilterConfig>("SupplierFilterConfig", new ListArgs<dynamic>(), VerifiedUserContext);
            return config.Reserialize<ListPage<SupplierFilterConfigDocument>>();
        }
    }
}
