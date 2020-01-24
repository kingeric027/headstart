using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Helpers;
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
	public interface IShippingCommand
	{
		Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, ShippingSelection shippingSelection);
		Task<IEnumerable<ShippingOptions>> GenerateShippingRatesAsync(string orderID);
	}

	public class ShippingCommand : IShippingCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IFreightPopService _freightPop;
		private readonly IMockShippingCacheService _shippingCache;

		public ShippingCommand(IFreightPopService freightPop, IMockShippingCacheService shippingCache)
		{
			_oc = OcFactory.GetSEBAdmin();
			_freightPop = freightPop;
			_shippingCache = shippingCache;
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
	}
}
