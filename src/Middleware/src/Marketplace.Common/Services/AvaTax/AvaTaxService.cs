using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services
{
	using Avalara.AvaTax.RestClient;
    using Marketplace.Common.Extensions;
    using Marketplace.Common.Models;
    using Marketplace.Common.Services.FreightPop;
    using OrderCloud.SDK;
	using System;
	using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

	public interface IAvataxService
	{
		// Use this before checkout. No records will be saved in avalara.
		Task<decimal> GetTaxEstimateAsync(TaxableOrder taxableOrder);
		// Use this during submit.
		Task<TransactionModel> CreateTransactionAsync(TaxableOrder taxableOrder);
		// Committing the transaction makes it eligible to be filed as part of a tax return. 
		// When should we do this? On order complete (When the credit card is charged) ? 
		Task<TransactionModel> CommitTaxTransactionAsync(string transactionCode);
	}

	public class AvataxService : IAvataxService
	{
		private readonly AvaTaxClient _avaTax;
		private readonly string companyCode = "Four51";

		public AvataxService(AppSettings settings)
		{
			//var env = settings.Env == AppEnvironment.Prod ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox;
			var env = AvaTaxEnvironment.Production;

			_avaTax = new AvaTaxClient("four51 marketplace", "v1", settings.Env.ToString(), env)
					.WithSecurity(settings.AvalaraSettings.AccountID, settings.AvalaraSettings.LicenseKey);
		}

		public async Task<decimal> GetTaxEstimateAsync(TaxableOrder taxableOrder)
		{
			var transaction = await CreateTransactionAsync(DocumentType.SalesOrder, taxableOrder);
			return transaction.totalTax ?? 0;
		}

		public async Task<TransactionModel> CreateTransactionAsync(TaxableOrder taxableOrder)
		{
			return await CreateTransactionAsync(DocumentType.SalesInvoice, taxableOrder);
		}

		private async Task<TransactionModel> CreateTransactionAsync(DocumentType docType, TaxableOrder taxableOrder)
		{
			var trans = new TransactionBuilder(_avaTax, companyCode, docType, GetCustomerCode(taxableOrder));
			//foreach (var line in taxableOrder.Lines) trans.WithLineItem(line);
			//foreach (var rate in taxableOrder.ShippingRates) trans.WithShippingRate(rate);

			return await trans.CreateAsync();
		}

		public async Task<TransactionModel> CommitTaxTransactionAsync(string transactionCode)
		{
			var model = new CommitTransactionModel() { commit = true };
			return await _avaTax.CommitTransactionAsync(companyCode, transactionCode, DocumentType.SalesInvoice, "", model);
		}

		private string GetCustomerCode(TaxableOrder taxableOrder)
		{
			return taxableOrder.Order.FromCompanyID;
		}

		#region Mock Avalara Response
		// fake, static data.In the correct format though.
		private readonly TransactionModel _mockTransaction = new TransactionModel()
		{
			id = 0,
			code = "63184372-8a62-421e-aa94-70373058f166(fake)",
			companyId = 835775,
			date = DateTime.Parse("2019-12-18"),
			paymentDate = DateTime.Parse("2019-12-18"),
			status = DocumentStatus.Temporary,
			type = DocumentType.SalesOrder,
			customerVendorCode = "ABC",
			customerCode = "ABC",
			reconciled = false,
			totalAmount = 40.21M,
			totalExempt = 0,
			totalDiscount = 0,
			totalTax = 3.11M,
			totalTaxable = 40.21M,
			totalTaxCalculated = 3.11M,
			adjustmentReason = AdjustmentReason.NotAdjusted,
			locked = false,
			version = 1,
			exchangeRateEffectiveDate = DateTime.Parse("2019-12-18"),
			exchangeRate = 1,
			modifiedDate = DateTime.Parse("2019-12-18T22:41:35.335454Z"),
			modifiedUserId = 247126,
			taxDate = DateTime.Parse("2019-12-18T00:00:00"),
			summary = new List<TransactionSummary> {
					new TransactionSummary() {
						country = "US",
						region = "CA",
						jurisType = JurisdictionType.Special,
						jurisCode = "EMTN0",
						jurisName = "ORANGE CO LOCAL TAX SL",
						taxAuthorityType = 45,
						stateAssignedNo = "30",
						taxType = TaxType.Sales,
						taxSubType = "S",
						taxName = "CA SPECIAL TAX",
						rateType = RateType.General,
						taxable = 40.21M,
						rate = 0.01M,
						tax = 0.4M,
						taxCalculated = 0.4M,
						nonTaxable = 0,
						exemption = 0
					}
				}
		};
		#endregion
	}
}
