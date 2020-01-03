using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services
{
	using Avalara.AvaTax.RestClient;
    using OrderCloud.SDK;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IAvataxService
	{
		Task<TransactionModel> GetTaxTransactionAsync(string transactionID); // Is this needed? for reporting maybe?
		Task<TransactionModel> CreateTaxTransactionAsync(Order order, ListPage<LineItem> lineItems);
		Task<TransactionModel> CommitTaxTransactionAsync(string transactionID);
	}

	public class AvataxService : IAvataxService
	{
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
		//private readonly AvaTaxClient _client;

		public AvataxService()
		{
			//var avaEnv = env.Contains("production") ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox;
			//_client = new AvaTaxClient("Four51 Marketplace", "v1", env, avaEnv)
			//		.WithSecurity(settings.AvalaraAccountID, settings.AvalaraLicenseKey);
		}

		public async Task<TransactionModel> GetTaxTransactionAsync(string transactionID)
		{
			// TODO - use Avarala endpoint
			return _mockTransaction;
		}

		public async Task<TransactionModel> CommitTaxTransactionAsync(string transactionID)
		{
			// TODO - use Avarala endpoint
			return _mockTransaction;
		}

		public async Task<TransactionModel> CreateTaxTransactionAsync(Order order, ListPage<LineItem> lineItems)
		{
			// TODO - use Avarala endpoint
			return _mockTransaction; 
		}
	}
}
