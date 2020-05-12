using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Common.Services.Avalara;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;

namespace Marketplace.Common.Controllers.Avalara
{
	[DocComments("\"Integration\" represents Avalara Tax Functionality")]
	[MarketplaceSection.Integration(ListOrder = 1)]
	[Route("avalara")]
	public class AvalaraController : BaseController
	{
		private readonly IAvalaraCommand _taxService;

		public AvalaraController(AppSettings settings, IAvalaraCommand taxService) : base(settings)
		{
			_taxService = taxService;
		}

		[DocName("List Tax Codes")]
		[HttpGet, Route("code"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<ListPage<TaxCode>> ListTaxCodes(ListArgs<TaxCode> marketplaceListArgs)
		{
			return await _taxService.ListTaxCodesAsync(marketplaceListArgs);
		}

		[DocName("Get tax exeption certificate details")]
		[HttpGet, Route("{companyID}/certificate/{certificateID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<TaxCertificate> GetCertificate(int companyID, int certificateID)
		{
			return await _taxService.GetCertificateAsync(companyID, certificateID);
		}

		[DocName("Create tax exeption certificate")]
		[HttpPost, Route("{companyID}/certificate"), OrderCloudIntegrationsAuth(ApiRole.AddressAdmin)]
		public async Task<TaxCertificate> CreateCertificate(int companyID, [FromBody] TaxCertificate cert)
		{
			return await _taxService.CreateCertificateAsync(companyID, cert);
		}

		[DocName("Update tax exeption certificate")]
		[HttpPut, Route("{companyID}/certificate/{certificateID}"), OrderCloudIntegrationsAuth(ApiRole.AddressAdmin)]
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
