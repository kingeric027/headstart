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
using System.Net.Http;
using System.Net;
using System.Text;

namespace Marketplace.Common.Controllers.Avalara
{
	[DocComments("\"Integration\" represents Avalara Tax Functionality")]
	[MarketplaceSection.Integration(ListOrder = 1)]
	[Route("avalara")]
	public class AvalaraController : BaseController
	{
		private readonly IAvalaraService _taxService;

		public AvalaraController(AppSettings settings, IAvalaraService taxService) : base(settings)
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
		[HttpGet, Route("{companyID}/certificate/{certificateID}"), MarketplaceUserAuth(ApiRole.Shopper)]
		public async Task<TaxCertificate> GetCertificate(int companyID, int certificateID)
		{
			return await _taxService.GetCertificateAsync(companyID, certificateID);
		}

		[DocName("Create tax exeption certificate")]
		[HttpPost, Route("{companyID}/certificate"), MarketplaceUserAuth(ApiRole.AddressAdmin)]
		public async Task<TaxCertificate> CreateCertificate(int companyID, [FromBody] TaxCertificate cert)
		{
			return await _taxService.CreateCertificateAsync(companyID, cert);
		}

		[DocName("Update tax exeption certificate")]
		[HttpPut, Route("{companyID}/certificate/{certificateID}"), MarketplaceUserAuth(ApiRole.AddressAdmin)]
		public async Task<TaxCertificate> UpdateCertificate(int companyID, int certificateID, [FromBody] TaxCertificate cert)
		{
			return await _taxService.UpdateCertificateAsync(companyID, certificateID, cert);
		}

		[HttpGet, Route("{companyID}/certificate/{certificateID}/pdf")]
		public async Task<object> DownloadCertificate(int companyID, int certificateID)
		{
			var pdf = await _taxService.DownloadCertificatePdfAsync(companyID, certificateID);
			return File(pdf, "application/pdf");
		}
	}
}
