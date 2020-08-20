using Avalara.AvaTax.RestClient;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.avalara
{
	public static class TransactionMapper
	{
		public static CreateTransactionModel ToAvalaraTransationModel(this OrderWorksheet order, string companyCode, DocumentType docType)
		{
			var lines = new List<LineItemModel>();
			foreach (var shipment in order.ShipEstimateResponse.ShipEstimates)
			{
				var (shipFrom, shipTo) = shipment.GetAddresses(order.LineItems);
				var method = shipment.GetSelectedShippingMethod();
				lines.Add(method.ToLineItemModel(shipFrom, shipTo));
			}

			var hasResaleCert = ((int?) order.Order.BillingAddress.xp.AvalaraCertificateID != null);
			var exemptionNo = hasResaleCert ? order.Order.BillingAddress.ID : null;

			foreach (var lineItem in order.LineItems) lines.Add(lineItem.ToLineItemModel(lineItem.ShipFromAddress, lineItem.ShippingAddress, exemptionNo));

			return new CreateTransactionModel()
			{
				companyCode = companyCode,
				type = docType,
				customerCode = order.Order.FromCompanyID,
				date = DateTime.Now,
				lines = lines
			};
		}


		private static LineItemModel ToLineItemModel(this LineItem lineItem, Address shipFrom, Address shipTo, string exemptionNo)
		{
			var line = new LineItemModel()
			{
				amount = lineItem.LineTotal,
				taxCode = lineItem.Product.xp.Tax.Code,
				itemCode = lineItem.ProductID,
				customerUsageType = null,
				number = lineItem.ID,
				addresses = ToAddressesModel(shipFrom, shipTo)
			};
			var isResaleProduct = (bool)lineItem.Product.xp.IsResale;
			if (isResaleProduct && exemptionNo != null)
			{
				line.exemptionCode = exemptionNo;
			}
			return line;
		}

		private static LineItemModel ToLineItemModel(this ShipMethod method, Address shipFrom, Address shipTo)
		{
			return new LineItemModel()
			{
				amount = method.Cost,
				taxCode = "FR",
				itemCode = method.Name,
				customerUsageType = null,
				number = method.ID,
				addresses = ToAddressesModel(shipFrom, shipTo)
			};
		}

		private static AddressesModel ToAddressesModel(Address shipFrom, Address shipTo)
		{
			return new AddressesModel()
			{
				shipFrom = shipFrom.ToAddressLocationInfo(),
				shipTo = shipTo.ToAddressLocationInfo(),
			};
		}

		private static AddressLocationInfo ToAddressLocationInfo(this Address address)
		{
			return new AddressLocationInfo()
			{
				line1 = address.Street1,
				line2 = address.Street2,
				city = address.City,
				region = address.State,
				postalCode = address.Zip,
				country = address.Country
			};
		}
	}
}
