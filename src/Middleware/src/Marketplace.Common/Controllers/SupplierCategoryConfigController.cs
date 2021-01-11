using Headstart.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Headstart.Models.Attributes;
using Headstart.Models.Extended;
using ordercloud.integrations.library;
using Headstart.Models;
using Newtonsoft.Json.Linq;
using Headstart.Common.Services.CMS;
using Headstart.Common.Services.CMS.Models;

namespace Headstart.Common.Controllers
{
    [DocComments("\"Supplier Filter Config\" represents Supplier Category Configuration")]
    [MarketplaceSection.Marketplace(ListOrder = 5)]
    public class SupplierFilterConfigController : BaseController
    {
        private readonly ICMSClient _cms;

        public SupplierFilterConfigController(AppSettings settings, ICMSClient cms) : base(settings)
        {
            _cms = cms;
        }

        [DocName("GET SupplierCategoryConfig")]
        [HttpGet, Route("/supplierfilterconfig"), OrderCloudIntegrationsAuth(ApiRole.Shopper, ApiRole.SupplierReader)]
        public async Task<ListPage<SupplierFilterConfigDocument>> Get()
        {
            var config = await _cms.Documents.List("SupplierFilterConfig", new ListArgs<Document<SupplierFilterConfig>>(), VerifiedUserContext.AccessToken);
            return config.Reserialize<ListPage<SupplierFilterConfigDocument>>();
        }
    }

    [SwaggerModel]
    // swagger generator can't handle composite models so alias into one
    public class SupplierFilterConfigDocument: Document<SupplierFilterConfig>
    {

    }
}
