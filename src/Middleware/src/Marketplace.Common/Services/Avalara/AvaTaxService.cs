using System.Linq;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Extensions;
using Marketplace.Common.Mappers.Avalara;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers;
using Marketplace.Models.Misc;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.Avalara
{
	public interface IAvataxService
	{
		// Use this before checkout. No records will be saved in avalara.
		Task<decimal> GetTaxEstimateAsync(OrderWorksheet orderWorksheet);
		// Use this during submit.
		Task<TransactionModel> CreateTransactionAsync(OrderWorksheet orderWorksheet);
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

		public async Task<decimal> GetTaxEstimateAsync(OrderWorksheet orderWorksheet)
		{
			var transaction = await CreateTransactionAsync(DocumentType.SalesOrder, orderWorksheet);
			return transaction.totalTax ?? 0;
		}

		public async Task<TransactionModel> CreateTransactionAsync(OrderWorksheet orderWorksheet)
		{
			return await CreateTransactionAsync(DocumentType.SalesInvoice, orderWorksheet);
		}

		private async Task<TransactionModel> CreateTransactionAsync(DocumentType docType, OrderWorksheet orderWorksheet)
		{
			var trans = new TransactionBuilder(_avaTax, _companyCode, docType, GetCustomerCode(orderWorksheet.Order));
			foreach (var shipmentEstimate in orderWorksheet.ShipEstimateResponse.ShipEstimates)
			{
				var selectedShipMethod = shipmentEstimate.ShipMethods.First(shipmentMethod => shipmentMethod.ID == shipmentEstimate.SelectedShipMethodID);
				var shippingRate = selectedShipMethod.Cost;
				var firstLineItemID = shipmentEstimate.ShipEstimateItems.FirstOrDefault().LineItemID;
				var firstLineItem = orderWorksheet.LineItems.First(lineItem => lineItem.ID == firstLineItemID);
				var shipFromAddress = firstLineItem.ShipFromAddress;
				var shipToAddress = firstLineItem.ShippingAddress;

				// This assumes the order has one ShipTo Address. Should change
				trans.WithShippingRate(shippingRate, shipFromAddress, shipToAddress);

			}
			
			foreach (var lineItem in orderWorksheet.LineItems) trans.WithLineItem(lineItem);

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
