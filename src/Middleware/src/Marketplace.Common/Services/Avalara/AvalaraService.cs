using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Extensions;
using Marketplace.Common.Services.Avalara.Mappers;
using Marketplace.Common.Services.AvaTax.Models;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Helpers;
using Marketplace.Models.Misc;
using Marketplace.Models.Models.Misc;
using OrderCloud.SDK;

namespace Marketplace.Common.Services.Avalara
{
	public interface IAvalaraService
	{
		Task<ListPage<TaxCode>> ListTaxCodesAsync(ListArgs<TaxCode> marketplaceListArgs);
		Task<TaxCertificate> GetCertificate(int companyID, int certifitcateID);
		Task<TaxCertificate> CreateCertificate(int companyID, TaxCertificate cert);
		Task<TaxCertificate> UpdateCertificate(int companyID, int certificateID, TaxCertificate cert);

		// Use this before checkout. No records will be saved in avalara.
		Task<decimal> GetEstimateAsync(OrderWorksheet orderWorksheet);
		// Use this during submit.
		Task<TransactionModel> CreateTransactionAsync(OrderWorksheet orderWorksheet);
		// Committing the transaction makes it eligible to be filed as part of a tax return. 
		// When should we do this? 
		Task<TransactionModel> CommitTaxTransactionAsync(string transactionCode);
	}

	public class AvalaraService : IAvalaraService
	{
		private readonly AvaTaxClient _avaTax;
		private readonly string _companyCode;

		public AvalaraService(AppSettings settings)
		{
			_companyCode = settings.AvalaraSettings.CompanyCode;
			var env = settings.Env == AppEnvironment.Prod ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox;

			_avaTax = new AvaTaxClient("four51 marketplace", "v1", settings.Env.ToString(), env)
					.WithSecurity(settings.AvalaraSettings.AccountID, settings.AvalaraSettings.LicenseKey);
		}

		public async Task<ListPage<TaxCode>> ListTaxCodesAsync(ListArgs<TaxCode> marketplaceListArgs)
		{
			var args = AvalaraMapper.Map(marketplaceListArgs);
			var avataxCodes = await _avaTax.ListTaxCodesAsync(args.Filter, args.Top, args.Skip, args.OrderBy);
			var codeList = AvalaraMapper.Map(avataxCodes, args);
			return codeList;
		}

		public async Task<TaxCertificate> GetCertificate(int companyID, int certifitcateID)
		{
			var certificate = await _avaTax.GetCertificateAsync(companyID, certifitcateID, "");
			return AvalaraMapper.Map(certificate);
		}

		public async Task<TaxCertificate> CreateCertificate(int companyID, TaxCertificate cert)
		{
			var certificates = await _avaTax.CreateCertificatesAsync(companyID, false, new List<CertificateModel> { AvalaraMapper.Map(cert) });
			return AvalaraMapper.Map(certificates[0]);
		}

		public async Task<TaxCertificate> UpdateCertificate(int companyID, int certifitcateID, TaxCertificate cert)
		{
			var certificate = await _avaTax.UpdateCertificateAsync(companyID, certifitcateID, AvalaraMapper.Map(cert));
			return AvalaraMapper.Map(certificate);
		}

		public async Task<decimal> GetEstimateAsync(OrderWorksheet orderWorksheet)
		{
			var taxEstimate = (await CreateTransactionAsync(DocumentType.SalesOrder, orderWorksheet)).totalTax ?? 0;
			return taxEstimate;
		}

		public async Task<TransactionModel> CreateTransactionAsync(OrderWorksheet orderWorksheet)
		{
			var transaction = await CreateTransactionAsync(DocumentType.SalesInvoice, orderWorksheet);
			return transaction;
		}

		public async Task<TransactionModel> CommitTaxTransactionAsync(string transactionCode)
		{
			var model = new CommitTransactionModel() { commit = true };
			var transaction = await _avaTax.CommitTransactionAsync(_companyCode, transactionCode, DocumentType.SalesInvoice, "", model);
			return transaction;
		}

		private string GetCustomerCode(Order order) => order.FromCompanyID;

		private async Task<TransactionModel> CreateTransactionAsync(DocumentType docType, OrderWorksheet orderWorksheet)
		{
			var trans = new TransactionBuilder(_avaTax, _companyCode, docType, GetCustomerCode(orderWorksheet.Order));
			trans.WithTransactionCode(orderWorksheet.Order.ID);
			foreach (var shipmentEstimate in orderWorksheet.ShipEstimateResponse.ShipEstimates)
			{
				var selectedShipMethod = shipmentEstimate.ShipMethods.First(shipmentMethod => shipmentMethod.ID == shipmentEstimate.SelectedShipMethodID);
				var shippingRate = selectedShipMethod.Cost;
				var firstLineItemID = shipmentEstimate.ShipEstimateItems.FirstOrDefault().LineItemID;
				var firstLineItem = orderWorksheet.LineItems.First(lineItem => lineItem.ID == firstLineItemID);
				var shipFromAddress = firstLineItem.ShipFromAddress;
				var shipToAddress = firstLineItem.ShippingAddress;

				// This assumes the order has one ShipTo Address. This is ok for marketplace, but not a general OC integration.
				trans.WithShippingRate(shippingRate, shipFromAddress, shipToAddress);
			}

			foreach (var lineItem in orderWorksheet.LineItems) trans.WithLineItem(lineItem);

			return await trans.CreateAsync();
		}
	}
}
