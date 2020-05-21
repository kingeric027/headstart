﻿using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Attributes;
using Marketplace.Models.Misc;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;
using Avalara.AvaTax.RestClient;
using ordercloud.integrations.avalara;
using Marketplace.Common.Commands;

namespace Marketplace.Common.Controllers.Avalara
{
	[DocComments("\"Integration\" represents Avalara Tax Functionality")]
	[MarketplaceSection.Integration(ListOrder = 1)]
	[Route("avalara")]
	public class AvalaraController : BaseController
	{
		private readonly IAvalaraCommand _avalara;
		private readonly IResaleCertCommand _resaleCertCommand;
		s
		public AvalaraController(AppSettings settings, IAvalaraCommand avalara, IResaleCertCommand resaleCertCommand, IOrderCloudClient oc) : base(settings)
		{
			_avalara = avalara;
			_resaleCertCommand = resaleCertCommand;
		}

		[DocName("Get Tax Estimate")]
		[HttpGet, Route("estimate"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<decimal> GetTaxEstimate([FromBody] OrderWorksheet orderWorksheet)
		{
			return await _avalara.GetEstimateAsync(orderWorksheet);
		}

		[DocName("Create Tax Transaction")]
		[HttpPost, Route("transaction"), OrderCloudIntegrationsAuth(ApiRole.OrderAdmin)]
		public async Task<TransactionModel> CreateTransaction([FromBody] OrderWorksheet orderWorksheet)
		{
			return await _avalara.CreateTransactionAsync(orderWorksheet);
		}

		[DocName("Commit Tax Transaction")]
		[HttpPost, Route("transaction/{transactionCode}/commit"), OrderCloudIntegrationsAuth(ApiRole.OrderAdmin)]
		public async Task<TransactionModel> CommitTransaction(string transactionCode)
		{
			return await _avalara.CommitTransactionAsync(transactionCode);
		}

		[DocName("List Tax Codes")]
		[HttpGet, Route("code"), OrderCloudIntegrationsAuth(ApiRole.ProductAdmin)]
		public async Task<ListPage<TaxCode>> ListTaxCodes(ListArgs<TaxCode> marketplaceListArgs)
		{
			return await _avalara.ListTaxCodesAsync(marketplaceListArgs);
		}

		[DocName("Get tax exeption certificate details")]
		[HttpGet, Route("{companyID}/certificate/{locationID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<TaxCertificate> GetCertificate(int companyID, string locationID)
		{
			return await _resaleCertCommand.GetAsync(companyID, locationID, VerifiedUserContext);
		}

		[DocName("Create tax exeption certificate")]
		[HttpPost, Route("{companyID}/certificate/{locationID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<TaxCertificate> CreateCertificate(int companyID, string locationID, [FromBody] TaxCertificate cert)
		{
			return await _resaleCertCommand.CreateAsync(companyID, locationID, cert, VerifiedUserContext);
		}

		[DocName("Update tax exeption certificate")]
		[HttpPut, Route("{companyID}/certificate/{locationID}"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
		public async Task<TaxCertificate> UpdateCertificate(int companyID, string locationID, [FromBody] TaxCertificate cert)
		{
			return await _resaleCertCommand.UpdateAsync(companyID, locationID, cert, VerifiedUserContext);
		}

		[HttpGet, Route("{companyID}/certificate/{locationID}/pdf")]
		public async Task<object> DownloadCertificate(int companyID, string locationID)
		{
			// need to include auth for managing cert for a specific location somewhere
			var pdf = await _avalara.DownloadCertificatePdfAsync(companyID, certificateID);
			return File(pdf, "application/pdf");
		}
	}
}
