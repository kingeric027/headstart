using Avalara.AvaTax.RestClient;
using Flurl;
using Flurl.Http;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ordercloud.integrations.library;

namespace ordercloud.integrations.avalara
{
	public interface IAvalaraCommand
	{
		// Use this before checkout. No records will be saved in avalara.
		Task<decimal> GetEstimateAsync(OrderWorksheet orderWorksheet);
		// Use this during submit.
		Task<TransactionModel> CreateTransactionAsync(OrderWorksheet orderWorksheet);
		// Committing the transaction makes it eligible to be filed as part of a tax return. 
		// When should we do this? 
		Task<TransactionModel> CommitTransactionAsync(string transactionCode);
		Task<ListPage<TaxCode>> ListTaxCodesAsync(ListArgs<TaxCode> marketplaceListArgs);
		Task<TaxCertificate> GetCertificateAsync(int companyID, int certificateID);
		Task<TaxCertificate> CreateCertificateAsync(int companyID, TaxCertificate cert);
		Task<TaxCertificate> UpdateCertificateAsync(int companyID, int certificateID, TaxCertificate cert);
		Task<byte[]> DownloadCertificatePdfAsync(int companyID, int certificateID);
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

		public async Task<ListPage<TaxCode>> ListTaxCodesAsync(ListArgs<TaxCode> marketplaceListArgs)
		{
			var args = TaxCodeMapper.Map(marketplaceListArgs);
			var avataxCodes = await _avaTax.ListTaxCodesAsync(args.Filter, args.Top, args.Skip, args.OrderBy);
			var codeList = TaxCodeMapper.Map(avataxCodes, args);
			return codeList;
		}

		public async Task<TaxCertificate> GetCertificateAsync(int companyID, int certificateID)
		{
			var certificate = await _avaTax.GetCertificateAsync(companyID, certificateID, "");
			var mappedCertificate = TaxCertificateMapper.Map(certificate, companyID, _settings.HostUrl);
			return mappedCertificate;
		}

		public async Task<TaxCertificate> CreateCertificateAsync(int companyID, TaxCertificate cert)
		{
			var certificates = await _avaTax.CreateCertificatesAsync(companyID, false, new List<CertificateModel> { TaxCertificateMapper.Map(cert) });
			var mappedCertificate = TaxCertificateMapper.Map(certificates[0], companyID, _settings.HostUrl);
			return mappedCertificate;
		}

		public async Task<TaxCertificate> UpdateCertificateAsync(int companyID, int certificateID, TaxCertificate cert)
		{
			var certificate = await _avaTax.UpdateCertificateAsync(companyID, certificateID, TaxCertificateMapper.Map(cert));
			var mappedCertificate = TaxCertificateMapper.Map(certificate, companyID, _settings.HostUrl);
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

		private async Task<TransactionModel> CreateTransactionAsync(DocumentType docType, OrderWorksheet orderWorksheet)
		{
			var items = orderWorksheet.LineItems;
			var shipments = orderWorksheet.ShipEstimateResponse.ShipEstimates;
			var builder = new TransactionBuilder(_avaTax, _companyCode, docType, GetCustomerCode(orderWorksheet.Order));
			builder.WithTransactionCode(orderWorksheet.Order.ID);
			foreach (var shipment in shipments)
			{
				var (shipFrom, shipTo) = shipment.GetAddresses(items);
				var method = shipment.GetSelectedShippingMethod();
				builder.WithShippingRate(method, shipFrom, shipTo);
			}

			foreach (var lineItem in items) builder.WithLineItem(lineItem);

			var transaction = await builder.CreateAsync();
			return transaction;
		}

		private string GetCustomerCode(Order order) => order.FromCompanyID;
	}
}
