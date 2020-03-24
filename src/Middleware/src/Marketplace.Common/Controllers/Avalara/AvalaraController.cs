using System;
using System.Collections.Generic;
using System.Text;
using Marketplace.Common.Commands;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Common.Services.Avalara;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Marketplace.Models.Models.Misc;
using Marketplace.Models.Misc;

namespace Marketplace.Common.Controllers.Avalara
{
	[DocComments("\"Integration\" represents Avalara Tax Functionality")]
	[MarketplaceSection.Integration(ListOrder = 1)]
	[Route("avalara")]
	public class AvalaraController : BaseController
	{
		private readonly IAvataxService _taxService;

		public AvalaraController(AppSettings settings, IAvataxService taxService) : base(settings)
		{
			_taxService = taxService;
		}

		[DocName("List Tax Codes")]
		[HttpGet, Route("code"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<ListPage<TaxCode>> ListTaxCodes(ListArgs<TaxCode> marketplaceListArgs)
		{
			return await _taxService.ListTaxCodesAsync(marketplaceListArgs);
		}

		[DocName("Get tax exeption certificate details")]
		[HttpGet, Route("{companyID}/certificate/{certificateID}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<TaxCertificate> GetCertificate(int companyID, int certificateID)
		{
			return await _taxService.GetCertificate(companyID, certificateID);
		}

		[DocName("Create tax exeption certificate")]
		[HttpPost, Route("{companyID}/certificate"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<TaxCertificate> CreateCertificate(int companyID, [FromBody] TaxCertificate cert)
		{
			return await _taxService.CreateCertificate(companyID, cert);
		}

		[DocName("Update tax exeption certificate")]
		[HttpPut, Route("{companyID}/certificate/{certificateID}"), MarketplaceUserAuth(ApiRole.ProductAdmin)]
		public async Task<TaxCertificate> UpdateCertificate(int companyID, int certificateID, [FromBody] TaxCertificate cert)
		{
			return await _taxService.UpdateCertificate(companyID, certificateID, cert);
		}

		[DocName("Download tax exeption certificate pdf")]
		[HttpGet, Route("{companyID}/certificate/{certificateID}/pdf"), MarketplaceUserAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
		public async Task DownloadCertificate(int companyID, int certificateID)
		{

		}
	}
}
