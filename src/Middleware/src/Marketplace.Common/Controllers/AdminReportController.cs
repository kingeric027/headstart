using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using Marketplace.Models.Attributes;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"test\" represents test for Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 99)]
    [Route("adminreports")]
    public class AdminReportController : BaseController
    {
        private readonly IMarketplaceAdminReportCommand _command;
        private readonly IOrderCloudClient _oc;

        public AdminReportController(IMarketplaceAdminReportCommand command, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _command = command;
            _oc = oc;
        }

        [HttpPost, Route("buyerlocationreport"), OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task<List<MarketplaceAddressBuyer>> BuyerLocationReport([FromBody] MarketplaceReportFilter filters)
        {
            return await _command.BuyerLocationReport(filters, VerifiedUserContext);
        }
    }
}
