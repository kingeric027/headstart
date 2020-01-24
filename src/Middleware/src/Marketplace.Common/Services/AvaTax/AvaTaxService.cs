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
		private readonly string _companyCode;

		public AvataxService(AppSettings settings)
		{
			_companyCode = settings.AvalaraSettings.CompanyCode;
			var env = settings.Env == AppEnvironment.Prod ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox;

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
			var trans = new TransactionBuilder(_avaTax, _companyCode, docType, GetCustomerCode(taxableOrder.Order));
			var shipments = taxableOrder.Lines.GroupBy(line => line.ShipFromAddressID); 
			foreach (var shipment in shipments)
			{
				taxableOrder.ShippingRates.TryGetValue(shipment.Key, out var shippingRate);
				// This assumes the order has one ShipTo Address. Should change
				trans.WithShippingRate(shippingRate, shipment.First().ShipFromAddress, shipment.First().ShippingAddress);
				foreach (var line in shipment) trans.WithLineItem(line);
			}

			return await trans.CreateAsync();
		}

		public async Task<TransactionModel> CommitTaxTransactionAsync(string transactionCode)
		{
			var model = new CommitTransactionModel() { commit = true };
			return await _avaTax.CommitTransactionAsync(_companyCode, transactionCode, DocumentType.SalesInvoice, "", model);
		}

		private string GetCustomerCode(Order order)
		{
			return order.FromCompanyID;
		}
	}
}
