﻿using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.DevCenter;
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
		Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, ShippingSelection shippingSelection);
		Task<MarketplaceOrder> CalcTaxAndPatchOrderAsync(string orderID);
		Task<IEnumerable<ShippingOptions>> GenerateShippingRatesAsync(string orderID);
	}

	public class OrderCheckoutCommand : IOrderCheckoutCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IFreightPopService _freightPop;
		private readonly IMockShippingCacheService _shippingCache;
		private readonly IAvataxService _avatax;

		public OrderCheckoutCommand(AppSettings settings, IAvataxService avatax, IFreightPopService freightPop, IMockShippingCacheService shippingCache)
		{
			// TODO - this authentication needs to be completely different. It needs to work accross ordercloud orgs. 
			_oc = new OrderCloudClient(new OrderCloudClientConfig()
			{
				ClientId = "2234C6E1-8FA5-41A2-8A7F-A560C6BA44D8",
				ClientSecret = "z08ibzgsb337ln8EzJx5efI1VKxqdqeBW0IB7p1SJaygloJ4J9uZOtPu1Aql",
				Roles = new[] { ApiRole.FullAccess }
			});
			_avatax = avatax;
			_freightPop = freightPop;
			_shippingCache = shippingCache;
		}

		public async Task<MarketplaceOrder> CalcTaxAndPatchOrderAsync(string orderID)
		{	
			var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
			var items = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);

			var inValid = ListShipmentsWithoutSelection(order, items.Items);
			Require.That(inValid.Count() == 0, ErrorCodes.Checkout.MissingShippingSelection, new MissingShippingSelectionError(inValid));

			var taxTransaction = await _avatax.CreateTaxTransactionAsync(order, items);
			
			return await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID, new PartialOrder()
			{
				TaxCost = taxTransaction.totalTax ?? 0,
				xp = new
				{
					AvalaraTaxTransactionCode = taxTransaction.code
				}
			});
		}

		public async Task<IEnumerable<ShippingOptions>> GenerateShippingRatesAsync(string orderID)
		{
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);
			var shipments = lineItems.Items.GroupBy(li => li.ShipFromAddressID);

			return await shipments.SelectAsync(async lineItemGrouping =>
			{
				var shipFrom = lineItemGrouping.First().ShipFromAddress;
				var shipTo = lineItemGrouping.First().ShippingAddress;
				var fPopResponse = await _freightPop.GetRatesAsync(shipFrom, shipTo, lineItemGrouping.ToList());
				await _shippingCache.SaveShippingRatesAsync(fPopResponse.Data.Rates);
				return new ShippingOptions()
				{
					SupplierID = lineItemGrouping.First().SupplierID, // Assumes ShipFromAddressID is unqiue accross suppliers
					ShipFromAddressID = lineItemGrouping.Key,
					Rates = fPopResponse.Data.Rates
				};
			});
		}

		public async Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, ShippingSelection selection)
		{
			var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);

			var exists = lineItems.Items.Any(li => li.ShipFromAddressID == selection.ShipFromAddressID);
			Require.That(exists, ErrorCodes.Checkout.InvalidShipFromAddress, new InvalidShipFromAddressIDError(selection.ShipFromAddressID));

			var selections = order.xp?.ShippingSelections?.ToDictionary(s => s.ShipFromAddressID) ?? new Dictionary<string, ShippingSelection> { };
			selections[selection.ShipFromAddressID] = selection;
			var totalShippingCost = (await selections
				.SelectAsync(async sel => await _shippingCache.GetSavedShippingRateAsync(orderID, sel.Value.ShippingRateID)))
				.Sum(savedQuote => savedQuote.TotalCost);

			return await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID, new PartialOrder()
			{
				ShippingCost = totalShippingCost,
				xp = new {
					ShippingSelections = selections.Values.ToArray()
				}
			});
		}

		private IEnumerable<string> ListShipmentsWithoutSelection(MarketplaceOrder order, IList<LineItem> items)
		{
			var shipFromAddressIDs = items.GroupBy(li => li.ShipFromAddressID).Select(group => group.Key);
			var selections = order.xp.ShippingSelections.Select(sel => sel.ShipFromAddressID);
			return shipFromAddressIDs.Except(selections);
		}
	}
}