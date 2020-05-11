using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalara.AvaTax.RestClient;
using Marketplace.Common.Extensions;
using Marketplace.Common.Services.Avalara.Mappers;
using Marketplace.Helpers.Models;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using Flurl.Http;
using Flurl;
using System.IO;
using Integrations.Avalara;
using Marketplace.Helpers;

namespace Marketplace.Common.Services.Avalara
{
	public interface IAvalaraCommand
	{
		Task<ListPage<TaxCode>> ListTaxCodesAsync(ListArgs<TaxCode> marketplaceListArgs);
		Task<TaxCertificate> GetCertificateAsync(int companyID, int certifitcateID);
		Task<TaxCertificate> CreateCertificateAsync(int companyID, TaxCertificate cert);
		Task<TaxCertificate> UpdateCertificateAsync(int companyID, int certificateID, TaxCertificate cert);
		Task<byte[]> DownloadCertificatePdfAsync(int companyID, int certificateID);
		// Use this before checkout. No records will be saved in avalara.
		Task<decimal> GetEstimateAsync(OrderWorksheet orderWorksheet);
		// Use this during submit.
		Task<TransactionModel> CreateTransactionAsync(OrderWorksheet orderWorksheet);
		// Committing the transaction makes it eligible to be filed as part of a tax return. 
		// When should we do this? 
		Task<TransactionModel> CommitTransactionAsync(string transactionCode);
	}

	public class AvalaraCommand : IAvalaraCommand
	{
		const string PROD_URL = "https://rest.avatax.com/api/v2";
		const string SANDBOX_URL = "https://sandbox-rest.avatax.com/api/v2";
		private readonly AvalaraConfig _settings;
		private readonly AvaTaxClient _avaTax;
		private readonly string _companyCode;
		private readonly string _baseUrl;

		public AvalaraCommand(AvalaraConfig settings)
		{
			_settings = settings;
			_companyCode = _settings.CompanyCode;
			_baseUrl = _settings.Env == AvaTaxEnvironment.Production ? PROD_URL : SANDBOX_URL;
			_avaTax = new AvaTaxClient("four51 marketplace", "v1", "machine_name", _settings.Env)
					.WithSecurity(_settings.AccountID, _settings.LicenseKey);
		}

		public async Task<ListPage<TaxCode>> ListTaxCodesAsync(ListArgs<TaxCode> marketplaceListArgs)
		{
			var args = AvalaraMapper.Map(marketplaceListArgs);
			var avataxCodes = await _avaTax.ListTaxCodesAsync(args.Filter, args.Top, args.Skip, args.OrderBy);
			var codeList = AvalaraMapper.Map(avataxCodes, args);
			return codeList;
		}

		public async Task<TaxCertificate> GetCertificateAsync(int companyID, int certifitcateID)
		{
			var certificate = await _avaTax.GetCertificateAsync(companyID, certifitcateID, "");
			var mappedCertificate = AvalaraMapper.Map(certificate, companyID, _settings.HostUrl);
			return mappedCertificate;
		}

		public async Task<TaxCertificate> CreateCertificateAsync(int companyID, TaxCertificate cert)
		{
			var certificates = await _avaTax.CreateCertificatesAsync(companyID, false, new List<CertificateModel> { AvalaraMapper.Map(cert) });
			var mappedCertificate = AvalaraMapper.Map(certificates[0], companyID, _settings.HostUrl);
			return mappedCertificate;
		}

		public async Task<TaxCertificate> UpdateCertificateAsync(int companyID, int certifitcateID, TaxCertificate cert)
		{
			var certificate = await _avaTax.UpdateCertificateAsync(companyID, certifitcateID, AvalaraMapper.Map(cert));
			var mappedCertificate = AvalaraMapper.Map(certificate, companyID, _settings.HostUrl);
			return mappedCertificate;
		}

		// The avalara SDK method for this was throwing an internal JSON parse exception.
		public async Task<byte[]> DownloadCertificatePdfAsync(int companyID, int certificateID)
		{
			var pdfBtyes = await new Url($"{_baseUrl}/companies/{companyID}/certificates/{certificateID}/attachment")
				.WithBasicAuth(_settings.AccountID.ToString(), _settings.LicenseKey)
				.GetBytesAsync();
			return pdfBtyes;
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

		public async Task<TransactionModel> CommitTransactionAsync(string transactionCode)
		{
			var model = new CommitTransactionModel() { commit = true };
			var transaction = await _avaTax.CommitTransactionAsync(_companyCode, transactionCode, DocumentType.SalesInvoice, "", model);
			return transaction;
		}

		private string GetCustomerCode(Order order) => order.FromCompanyID;

		private async Task<TransactionModel> CreateTransactionAsync(DocumentType docType, OrderWorksheet orderWorksheet)
		{
			var builder = new TransactionBuilder(_avaTax, _companyCode, docType, GetCustomerCode(orderWorksheet.Order));
			builder.WithTransactionCode(orderWorksheet.Order.ID);
			foreach (var shipmentEstimate in orderWorksheet.ShipEstimateResponse.ShipEstimates)
			{
				var selectedShipMethod = shipmentEstimate.ShipMethods.First(shipmentMethod => shipmentMethod.ID == shipmentEstimate.SelectedShipMethodID);
				var shippingRate = selectedShipMethod.Cost;
				var firstLineItemID = shipmentEstimate.ShipEstimateItems.FirstOrDefault().LineItemID;
				var firstLineItem = orderWorksheet.LineItems.First(lineItem => lineItem.ID == firstLineItemID);
				var shipFromAddress = firstLineItem.ShipFromAddress;
				var shipToAddress = firstLineItem.ShippingAddress;

				// This assumes the order has one ShipTo Address. This is ok for marketplace, but not a general OC integration.
				builder.WithShippingRate(shippingRate, shipFromAddress, shipToAddress);
			}

			foreach (var lineItem in orderWorksheet.LineItems) builder.WithLineItem(lineItem);

			var transaction = await builder.CreateAsync();
			return transaction;
		}
	}
}
