using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Helpers;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
	public interface ITaxCommand
	{
		Task<MarketplaceOrder> ApplyTaxEstimate(string orderID);
	}

	public class TaxCommand: ITaxCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IMockShippingCacheService _shippingCache;
		private readonly IAvataxService _avatax;

		public TaxCommand(IAvataxService avatax, IMockShippingCacheService shippingCache)
		{
			_oc = OcFactory.GetSEBAdmin();
			_avatax = avatax;
			_shippingCache = shippingCache;
		}

		public async Task<MarketplaceOrder> ApplyTaxEstimate(string orderID)
		{
			var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
			var items = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);

			var inValid = ListShipmentsWithoutSelection(order, items.Items);
			Require.That(inValid.Count() == 0, Exceptions.ErrorCodes.Checkout.MissingShippingSelection, new MissingShippingSelectionError(inValid));

			var shippingSelection = new Dictionary<string, ShippingRate>();
			foreach (var selection in order.xp.ShippingSelections)
			{
				var rate = await _shippingCache.GetSavedShippingRateAsync(orderID, selection.ShippingRateID);
				shippingSelection.Add(selection.ShipFromAddressID, rate);
			}

			var taxOrder = new TaxableOrder() { Order = order, Lines = items.Items, ShippingRates = shippingSelection };

			var totalTax = await _avatax.GetTaxEstimateAsync(taxOrder);

			return await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID, new PartialOrder()
			{
				TaxCost = totalTax
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
