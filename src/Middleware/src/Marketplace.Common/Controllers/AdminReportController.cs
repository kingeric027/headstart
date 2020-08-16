﻿using Marketplace.Common.Commands;
using Marketplace.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using ordercloud.integrations.library;
using Marketplace.Models.Attributes;
using Marketplace.Common.Models;
using Marketplace.Models.Misc;
using static Marketplace.Common.Models.ReportTemplate;

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

        [HttpGet, Route("buyerLocation/preview/{templateID}"), OrderCloudIntegrationsAuth]
        public async Task<List<MarketplaceAddressBuyer>> BuyerLocation(string templateID)
        {
            RequireOneOf(CustomRole.MPSupplierAdmin);
            return await _reportDataCommand.BuyerLocation(templateID, VerifiedUserContext);
        }

        [HttpPost, Route("buyerLocation/download/{templateID}"), OrderCloudIntegrationsAuth]
        public async Task DownloadBuyerLocation([FromBody] ReportRequestBody requestBody, string templateID)
        {
            RequireOneOf(CustomRole.MPSupplierAdmin);
            var reportData = await _reportDataCommand.BuyerLocation(templateID, VerifiedUserContext);
            await _downloadReportCommand.ExportToExcel(ReportTypeEnum.BuyerLocation, requestBody.Headers, reportData);

        }

        [HttpGet, Route("{reportType}/listtemplates"), OrderCloudIntegrationsAuth]
        public async Task<List<ReportTemplate>> ListReportTemplatesByReportType(ReportTypeEnum reportType)
        {
            RequireOneOf(CustomRole.MPSupplierAdmin);
            return await _reportDataCommand.ListReportTemplatesByReportType(reportType, VerifiedUserContext);
        }

        [HttpPost, Route("{reportType}"), OrderCloudIntegrationsAuth(ApiRole.AdminUserAdmin)]
        public async Task<ReportTemplate> PostReportTemplate([FromBody] ReportTemplate reportTemplate)
        {
            RequireOneOf(CustomRole.MPReportReader);
            return await _reportDataCommand.PostReportTemplate(reportTemplate, VerifiedUserContext);
        }

        [HttpDelete, Route("{id}"), OrderCloudIntegrationsAuth(ApiRole.AdminUserAdmin)]
        public async Task DeleteReportTemplate(string id)
        {
            RequireOneOf(CustomRole.MPReportReader);
            await _reportDataCommand.DeleteReportTemplate(id, VerifiedUserContext);
        }
    }
}
