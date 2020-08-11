using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using ordercloud.integrations.library;
using Marketplace.Models.Attributes;
using Marketplace.Common.Models;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Reports\" for generating and downloading reports in the Admin application")]
    [MarketplaceSection.Marketplace(ListOrder = 11)]
    [Route("reports")]
    public class AdminReportController : BaseController
    {
        private readonly IMarketplaceAdminReportCommand _reportDataCommand;
        private readonly DownloadReportCommand _downloadReportCommand;

        public AdminReportController(IMarketplaceAdminReportCommand reportDataCommand, AppSettings settings, DownloadReportCommand downloadReportCommand) : base(settings)
        {
            _reportDataCommand = reportDataCommand;
            _downloadReportCommand = downloadReportCommand;
        }

        public class ReportRequestBody
        {
            public string[] Headers { get; set; }
        }

        [HttpGet, Route("buyerLocation/preview/{templateID}"), OrderCloudIntegrationsAuth(ApiRole.SupplierReader, ApiRole.AdminUserAdmin)]
        public async Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID)
        {
            return await _reportDataCommand.BuyerLocation(templateID, VerifiedUserContext);
        }

        [HttpPost, Route("buyerLocation/download/{templateID}"), OrderCloudIntegrationsAuth(ApiRole.SupplierReader, ApiRole.AdminUserAdmin)]
        public async Task DownloadBuyerLocation([FromBody] ReportRequestBody requestBody, string templateID)
        {
            var reportData = await _reportDataCommand.BuyerLocation(templateID, VerifiedUserContext);
            await _downloadReportCommand.ExportToExcel("BuyerLocation", requestBody.Headers, reportData);

        }

        [HttpGet, Route("{reportType}/listtemplates"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
        public async Task<List<ReportTemplate>> ListReportTemplatesByReportType(string reportType)
        {
            return await _reportDataCommand.ListReportTemplatesByReportType(reportType, VerifiedUserContext);
        }

        [HttpPost, Route("{reportType}"), OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task<ReportTemplate> PostReportTemplate([FromBody] ReportTemplate reportTemplate)
        {
            return await _reportDataCommand.PostReportTemplate(reportTemplate, VerifiedUserContext);
        }

        [HttpDelete, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.BuyerAdmin)]
        public async Task DeleteReportTemplate(string id)
        {
            await _reportDataCommand.DeleteReportTemplate(id, VerifiedUserContext);
        }
    }
}
