using System.Linq;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Extensions;
using Marketplace.Common.Mappers.Avalara;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Helpers;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.AvaTax
{
    public interface IAvataxService
	{
		// Use this before checkout. No records will be saved in avalara.
		Task<decimal> GetTaxEstimateAsync(TaxableOrder taxableOrder);
		// Use this during submit.
		Task<TransactionModel> CreateTransactionAsync(TaxableOrder taxableOrder);
		// Committing the transaction makes it eligible to be filed as part of a tax return. 
		// When should we do this? On order complete (When the credit card is charged) ? 
		Task<TransactionModel> CommitTaxTransactionAsync(string transactionCode);
		Task<ListPage<MarketplaceTaxCode>> ListTaxCodesAsync(ListArgs<TaxCodeModel> marketplaceListArgs);
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
		public async Task<ListPage<MarketplaceTaxCode>> ListTaxCodesAsync(ListArgs<TaxCodeModel> marketplaceListArgs)
		{
			var taxCategory = marketplaceListArgs.Filters[0].Values[0].Term;
			var taxCategorySearch = marketplaceListArgs.Filters[0].Values[0].Term.Trim('0');
			var search = marketplaceListArgs.Search;
			var avataxQuery = search != "" ? $"isActive eq true and taxCode startsWith '{taxCategorySearch}' and (taxCode contains '{search}' OR description contains '{search}')" : $"isActive eq true and taxCode startsWith '{taxCategorySearch}'";
			var (top, skip) = TaxCodeMapper.Map(marketplaceListArgs.Page, marketplaceListArgs.PageSize);
			var avataxCodes = await _avaTax.ListTaxCodesAsync(avataxQuery, top, skip, null);
			var marketplaceTaxCodeList = TaxCodeMapper.Map(avataxCodes, taxCategory);
			return TaxCodeMapper.Map(avataxCodes, marketplaceTaxCodeList, top, skip);
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

		private string GetCustomerCode(OrderCloud.SDK.Order order)
		{
			return order.FromCompanyID;
		}
	}
}
