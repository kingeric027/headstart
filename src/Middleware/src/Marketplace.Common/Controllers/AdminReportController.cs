using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using Marketplace.Models.Attributes;
using Newtonsoft.Json.Linq;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"test\" represents test for Marketplace")]
    [MarketplaceSection.Marketplace(ListOrder = 99)]
    [Route("adminreports")]
    public class AdminReportController : BaseController
    {
        private readonly IMarketplaceAdminReportCommand _command;
        private readonly IOrderCloudClient _oc;
        private readonly DownloadReportCommand _downloadReportCommand;

        public AdminReportController(IMarketplaceAdminReportCommand command, IOrderCloudClient oc, AppSettings settings, DownloadReportCommand downloadReportCommand) : base(settings)
        {
            _command = command;
            _oc = oc;
            _downloadReportCommand = downloadReportCommand;
        }

        [HttpPost, Route("generate-report")]

        public async Task GenerateReport([FromBody] ReportRequestBody requestBody)
        {
            await _downloadReportCommand.ExportToExcel(requestBody.ReportType, requestBody.Headers, requestBody.Data);
        }

        public class ReportRequestBody
        {
            public string ReportType { get; set; }
            public string[] Headers { get; set; }
            public IEnumerable<JObject> Data { get; set; }
        }

        [HttpPost, Route("buyerlocationreport"), OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task<List<MarketplaceAddressBuyer>> BuyerLocationReport([FromBody] MarketplaceReportFilter filters)
        {
            return await _command.BuyerLocationReport(filters, VerifiedUserContext);
        }
    }
}
