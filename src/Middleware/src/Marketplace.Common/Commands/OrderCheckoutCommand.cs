using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.Winmark.Common.Services;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
	public interface IOrderCheckoutCommand
	{
		Task<Order> SetShippingAndTax(string orderID, IEnumerable<ShippingSelectionsFromOneAddress> shippingSelections);
		Task<IEnumerable<ShippingOptionsFromOneAddress>> GenerateShippingQuotes(string orderID);
	}

	public class OrderCheckoutCommand : IOrderCheckoutCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IMockShippingService _shipping;
		private readonly IAvataxService _avatax;

		public OrderCheckoutCommand(IOrderCloudClient oc, IAvataxService avatax, IMockShippingService shipping)
		{
			_oc = oc;
			_avatax = avatax;
			_shipping = shipping;
		}



		public async Task<Order> SetShippingAndTax(string orderID, IEnumerable<ShippingSelectionsFromOneAddress> shippingSelections)
		{	
			var order = await _oc.Orders.GetAsync(OrderDirection.Outgoing, orderID);
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderID);

			var taxTransaction = await _avatax.CreateTaxTransactionAsync(order, lineItems);
			var shippingCost = (await shippingSelections
				.SelectAsync(async sel => await _shipping.GetSavedShipmentQuote(orderID, sel.ShippingQuoteID)))
				.Sum(quote => quote.Cost);

			var patchOrder = new PartialOrder()
			{
				ShippingCost = shippingCost,
				TaxCost = taxTransaction.totalTax ?? 0,
				xp = new {
					ShippingSelections = shippingSelections,  // TODO - These xp's are placeholders until we actually define them.
					AvalaraTaxTransactionCode = taxTransaction.code
				}
			};

			return await _oc.Orders.PatchAsync(OrderDirection.Outgoing, orderID, patchOrder);
		}

		public async Task<IEnumerable<ShippingOptionsFromOneAddress>> GenerateShippingQuotes(string orderID)
		{
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderID);
			var shipments = lineItems.Items.GroupBy(li => li.ShipFromAddressID);
			return await shipments.SelectAsync(async lineItemGrouping =>
			{
				var lineItemsInShipment = lineItemGrouping.ToList();
				var options = await _shipping.GenerateShipmentQuotes(lineItemsInShipment);
				return new ShippingOptionsFromOneAddress()
				{
					SupplierID = lineItemsInShipment.First().SupplierID, // Assumes ShipFromAddressID is unqiue accross suppliers
					ShipFromAddressID = lineItemGrouping.Key,
					Options = options
				};
			});
		}
	}
}
