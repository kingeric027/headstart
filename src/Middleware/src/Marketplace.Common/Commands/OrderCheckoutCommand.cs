using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Helpers;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorCodes = Marketplace.Common.Exceptions.ErrorCodes;

namespace Marketplace.Common.Commands
{
	public interface IOrderCheckoutCommand
	{
		Task<MarketplaceOrder> SetShippingSelection(string orderID, ShippingSelection shippingSelection);
		Task<MarketplaceOrder> CalculateTax(string orderID);
		Task<IEnumerable<ShippingOptions>> GenerateShippingQuotes(string orderID);
	}

	public class OrderCheckoutCommand : IOrderCheckoutCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IMockShippingService _shipping;
		private readonly IAvataxService _avatax;

		public OrderCheckoutCommand(AppSettings settings, IAvataxService avatax, IMockShippingService shipping)
		{
			_oc = new OrderCloudClient(new OrderCloudClientConfig()
			{
				ApiUrl = settings.OrderCloudSettings.ApiUrl,
				AuthUrl = settings.OrderCloudSettings.AuthUrl,
				ClientId = "F1631510-F165-4278-A4EB-E655C5AC2E06"
			});
			_avatax = avatax;
			_shipping = shipping;
		}

		public async Task<MarketplaceOrder> CalculateTax(string orderID)
		{	
			var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Outgoing, orderID);
			var items = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderID);

			var inValid = ListShipmentsWithoutSelection(order, items.Items);
			Require.That(inValid.Count() == 0, ErrorCodes.Checkout.MissingShippingSelection, new MissingShippingSelectionError(inValid));

			var taxTransaction = await _avatax.CreateTaxTransactionAsync(order, items);
			
			return await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Outgoing, orderID, new PartialOrder()
			{
				TaxCost = taxTransaction.totalTax ?? 0,
				xp = new
				{
					AvalaraTaxTransactionCode = taxTransaction.code
				}
			});
		}

		public async Task<IEnumerable<ShippingOptions>> GenerateShippingQuotes(string orderID)
		{
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderID);
			var shipments = lineItems.Items.GroupBy(li => li.ShipFromAddressID);
			return await shipments.SelectAsync(async lineItemGrouping =>
			{
				var lineItemsInShipment = lineItemGrouping.ToList();
				var options = await _shipping.GenerateShipmentQuotes(lineItemsInShipment);
				return new ShippingOptions()
				{
					SupplierID = lineItemsInShipment.First().SupplierID, // Assumes ShipFromAddressID is unqiue accross suppliers
					ShipFromAddressID = lineItemGrouping.Key,
					Quotes = options
				};
			});
		}

		public async Task<MarketplaceOrder> SetShippingSelection(string orderID, ShippingSelection selection)
		{
			var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Outgoing, orderID);
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderID);

			var valid = lineItems.Items.Any(li => li.ShipFromAddressID == selection.ShipFromAddressID);
			Require.That(valid, ErrorCodes.Checkout.InvalidShipFromAddress, new InvalidShipFromAddressIDError(selection.ShipFromAddressID));

			order.xp.ShippingSelections[selection.ShipFromAddressID] = selection;
			var totalCost = (await order.xp.ShippingSelections
				.SelectAsync(async sel => await _shipping.GetSavedShipmentQuote(orderID, sel.Value.ShippingQuoteID)))
				.Sum(savedQuote => savedQuote.Cost);

			return await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID, new PartialOrder()
			{
				ShippingCost = totalCost,
				xp = new {
					ShippingSelections = order.xp.ShippingSelections
				}
			});
		}

		private IEnumerable<string> ListShipmentsWithoutSelection(MarketplaceOrder order, IList<LineItem> items)
		{
			var shipFromAddressIDs = items.GroupBy(li => li.ShipFromAddressID).Select(group => group.Key);
			var selections = order.xp.ShippingSelections.Select(sel => sel.Value.ShipFromAddressID);
			return shipFromAddressIDs.Except(selections);
		}
	}
}
