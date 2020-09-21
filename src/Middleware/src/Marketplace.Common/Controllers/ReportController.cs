using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using ordercloud.integrations.library;
using Marketplace.Models.Attributes;
using Marketplace.Common.Models;
using Marketplace.Models.Misc;
using Marketplace.Common.Models.Marketplace;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Marketplace Reports\" for generating and downloading reports in the Admin application")]
    [MarketplaceSection.Marketplace(ListOrder = 11)]
    [Route("reports")]
    public class ReportController : BaseController
    {
        private readonly IMarketplaceReportCommand _reportDataCommand;
        private readonly DownloadReportCommand _downloadReportCommand;

        public ReportController(IMarketplaceReportCommand reportDataCommand, AppSettings settings, DownloadReportCommand downloadReportCommand) : base(settings)
        {
            _reportDataCommand = reportDataCommand;
            _downloadReportCommand = downloadReportCommand;
        }

        public class ReportRequestBody
        {
            public string[] Headers { get; set; }
        }

        [HttpGet, Route("fetchAllReportTypes"), OrderCloudIntegrationsAuth]
        public ListPage<ReportTypeResource> FetchAllReportTypes()
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            return _reportDataCommand.FetchAllReportTypes(VerifiedUserContext);
        }

        [HttpGet, Route("BuyerLocation/preview/{templateID}"), OrderCloudIntegrationsAuth]
        public async Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            return await _reportDataCommand.BuyerLocation(templateID, VerifiedUserContext);
        }

        [HttpPost, Route("BuyerLocation/download/{templateID}"), OrderCloudIntegrationsAuth]
        public async Task<string> DownloadBuyerLocation([FromBody] ReportTemplate reportTemplate, string templateID)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            var reportData = await _reportDataCommand.BuyerLocation(templateID, VerifiedUserContext);
            return await _downloadReportCommand.ExportToExcel(ReportTypeEnum.BuyerLocation, reportTemplate, reportData);

        }

        [HttpGet, Route("SalesOrderDetail/preview/{templateID}/{lowDateRange}/{highDateRange}"), OrderCloudIntegrationsAuth]
        public async Task<List<MarketplaceOrder>> SalesOrderDetail(string templateID, string lowDateRange, string highDateRange)
        {
            RequireOneOf(CustomRole.MPReportAdmin);
            return await _reportDataCommand.SalesOrderDetail(templateID, lowDateRange, highDateRange, VerifiedUserContext);
        }

        [HttpPost, Route("SalesOrderDetail/download/{templateID}/{lowDateRange}/{highDateRange}"), OrderCloudIntegrationsAuth]
        public async Task<string> DownloadSalesOrderDetail([FromBody] ReportTemplate reportTemplate, string templateID, string lowDateRange, string highDateRange)
        {
            RequireOneOf(CustomRole.MPReportAdmin);
            var reportData = await _reportDataCommand.SalesOrderDetail(templateID, lowDateRange, highDateRange, VerifiedUserContext);
            return await _downloadReportCommand.ExportToExcel(ReportTypeEnum.SalesOrderDetail, reportTemplate, reportData);

        }

        [HttpGet, Route("PurchaseOrderDetail/preview/{templateID}/{lowDateRange}/{highDateRange}"), OrderCloudIntegrationsAuth]
        public async Task<List<MarketplaceOrder>> PurchaseOrderDetail(string templateID, string lowDateRange, string highDateRange)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            return await _reportDataCommand.PurchaseOrderDetail(templateID, lowDateRange, highDateRange, VerifiedUserContext);
        }

        [HttpPost, Route("PurchaseOrderDetail/download/{templateID}/{lowDateRange}/{highDateRange}"), OrderCloudIntegrationsAuth]
        public async Task<string> DownloadPurchaseOrderDetail([FromBody] ReportTemplate reportTemplate, string templateID, string lowDateRange, string highDateRange)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            var reportData = await _reportDataCommand.PurchaseOrderDetail(templateID, lowDateRange, highDateRange, VerifiedUserContext);
            return await _downloadReportCommand.ExportToExcel(ReportTypeEnum.PurchaseOrderDetail, reportTemplate, reportData);

        }

        [HttpGet, Route("LineItemDetail/preview/{templateID}/{lowDateRange}/{highDateRange}"), OrderCloudIntegrationsAuth]
        public async Task<List<MarketplaceLineItemOrder>> LineItemDetail(string templateID, string lowDateRange, string highDateRange)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            return await _reportDataCommand.LineItemDetail(templateID, lowDateRange, highDateRange, VerifiedUserContext);
        }

        [HttpPost, Route("LineItemDetail/download/{templateID}/{lowDateRange}/{highDateRange}"), OrderCloudIntegrationsAuth]
        public async Task<string> DownloadLineItemDetail([FromBody] ReportTemplate reportTemplate, string templateID, string lowDateRange, string highDateRange)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            var reportData = await _reportDataCommand.LineItemDetail(templateID, lowDateRange, highDateRange, VerifiedUserContext);
            return await _downloadReportCommand.ExportToExcel(ReportTypeEnum.LineItemDetail, reportTemplate, reportData);
        }

        [HttpGet, Route("download-shared-access/{fileName}"), OrderCloudIntegrationsAuth]
        public string GetSharedAccessSignature(string fileName)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            return _downloadReportCommand.GetSharedAccessSignature(fileName);
        }

        [HttpGet, Route("{reportType}/listtemplates"), OrderCloudIntegrationsAuth]
        public async Task<List<ReportTemplate>> ListReportTemplatesByReportType(ReportTypeEnum reportType)
        {
            RequireOneOf(CustomRole.MPReportReader, CustomRole.MPReportAdmin);
            return await _reportDataCommand.ListReportTemplatesByReportType(reportType, VerifiedUserContext);
        }

        [HttpPost, Route("{reportType}"), OrderCloudIntegrationsAuth(ApiRole.AdminUserAdmin)]
        public async Task<ReportTemplate> PostReportTemplate(ReportTypeEnum reportType, [FromBody] ReportTemplate reportTemplate)
        {
            RequireOneOf(CustomRole.MPReportAdmin);
            return await _reportDataCommand.PostReportTemplate(reportTemplate, VerifiedUserContext);
        }

        [HttpGet, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.AdminUserAdmin)]
        public async Task<ReportTemplate> GetReportTemplate(string id)
        {
            RequireOneOf(CustomRole.MPReportAdmin);
            return await _reportDataCommand.GetReportTemplate(id, VerifiedUserContext);
        }

        [HttpPut, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.AdminUserAdmin)]
        public async Task<ReportTemplate> UpdateReportTemplate(string id, [FromBody] ReportTemplate reportTemplate)
        {
            RequireOneOf(CustomRole.MPReportAdmin);
            return await _reportDataCommand.UpdateReportTemplate(id, reportTemplate, VerifiedUserContext);
        }

        [HttpDelete, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.AdminUserAdmin)]
        public async Task DeleteReportTemplate(string id)
        {
            RequireOneOf(CustomRole.MPReportAdmin);
            await _reportDataCommand.DeleteReportTemplate(id);
        }
    }
}
