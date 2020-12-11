using Marketplace.Common.Queries;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using Marketplace.Models.Extended;
using ordercloud.integrations.library;
using Marketplace.Models;
using Newtonsoft.Json.Linq;
using Marketplace.Common.Services.CMS;
using Marketplace.Common.Services.CMS.Models;

namespace Marketplace.Common.Controllers
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
        public async Task<ListPage<Document<SupplierFilterConfig>>> Get()
        {
            var config = await _cms.Documents.List("SupplierFilterConfig", new ListArgs<Document<SupplierFilterConfig>>(), VerifiedUserContext.AccessToken);
            return config.Reserialize<ListPage<Document<SupplierFilterConfig>>>();
        }
    }
}
