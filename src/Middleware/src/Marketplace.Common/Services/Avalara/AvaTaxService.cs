using System;
using System.Collections.Generic;
using System.Text;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Extensions;
using Marketplace.Common.Mappers.Avalara;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers;
using OrderCloud.SDK;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Common.Services.AvaTax
{
	public interface IAvataxService
	{
		// Use this before checkout. No records will be saved in avalara.
		Task<decimal> GetTaxEstimateAsync(OrderCalculation taxableOrder);
		// Use this during submit.
		Task<TransactionModel> CreateTransactionAsync(OrderCalculation taxableOrder);
		// Committing the transaction makes it eligible to be filed as part of a tax return. 
		// When should we do this? 
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
			var args = TaxCodeMapper.Map(marketplaceListArgs);
			var avataxCodes = await _avaTax.ListTaxCodesAsync(args.Filter, args.Top, args.Skip, args.OrderBy);
			var codeList = TaxCodeMapper.Map(avataxCodes, args);
			return codeList;
		}

		public async Task<decimal> GetTaxEstimateAsync(OrderCalculation orderCalculation)
		{
			var transaction = await CreateTransactionAsync(DocumentType.SalesOrder, orderCalculation);
			return transaction.totalTax ?? 0;
		}

		public async Task<TransactionModel> CreateTransactionAsync(OrderCalculation orderCalculation)
		{
			return await CreateTransactionAsync(DocumentType.SalesInvoice, orderCalculation);
		}

		private async Task<TransactionModel> CreateTransactionAsync(DocumentType docType, OrderCalculation orderCalculation)
		{
			var trans = new TransactionBuilder(_avaTax, _companyCode, docType, GetCustomerCode(orderCalculation.Order));
			foreach (var proposedShipment in orderCalculation.ProposedShipmentRatesResponse)
			{
				var selectedProposedShipment = proposedShipment.ProposedShipmentOptions.First(proposedShipmentOption => proposedShipmentOption.ID == proposedShipment.SelectedProposedShipmentOptionID);
				var shippingRate = selectedProposedShipment.Cost;
				var firstLineItemID = proposedShipment.ProposedShipmentItems.FirstOrDefault().LineItemID;
				var firstLineItem = orderCalculation.LineItems.First(lineItem => lineItem.ID == firstLineItemID);
				var shipFromAddress = firstLineItem.ShipFromAddress;
				var shipToAddress = firstLineItem.ShippingAddress;

				// This assumes the order has one ShipTo Address. Should change
				trans.WithShippingRate(shippingRate, shipFromAddress, shipToAddress);

				foreach (var lineItem in orderCalculation.LineItems) trans.WithLineItem(lineItem);
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
