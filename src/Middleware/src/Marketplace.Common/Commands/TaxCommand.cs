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
		Task HandleTransactionCreation(MarketplaceOrder buyerOrder);
	}

	public class TaxCommand: ITaxCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IAvataxService _avatax;

		public TaxCommand(IAvataxService avatax)
		{
			_oc = OcFactory.GetSEBAdmin();
			_avatax = avatax;
		}

		public async Task<MarketplaceOrder> ApplyTaxEstimate(string orderID)
		{
			/* once the platform proposed shipment functionality is complete
			 * this section will be rewritten and the rate will not be stored on the order */

			// change back to marketplace order once the model is updated
			var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
			var taxableOrder = await GetTaxableOrder(order);
			var inValid = ListShipmentsWithoutSelection(order, taxableOrder.Lines);
			Require.That(inValid.Count() == 0 || inValid == null, Exceptions.ErrorCodes.Checkout.MissingShippingSelection, new MissingShippingSelectionError(inValid));

			var totalTax = await _avatax.GetTaxEstimateAsync(taxableOrder);

			return await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID, new PartialOrder()
			{
				TaxCost = totalTax
			});
		}

		public async Task HandleTransactionCreation(MarketplaceOrder buyerOrder)
		{
			var taxableOrder = await GetTaxableOrder(buyerOrder);
			await _avatax.CreateTransactionAsync(taxableOrder);
		}

		private async Task<TaxableOrder> GetTaxableOrder(MarketplaceOrder order)
		{
			var items = await _oc.LineItems.ListAsync(OrderDirection.Incoming, order.ID);
			
			var shippingSelection = new Dictionary<string, decimal>();
			foreach (var selection in order.xp.ProposedShipmentSelections)
			{
				shippingSelection.Add(selection.ShipFromAddressID, selection.Rate);
			}

			return new TaxableOrder() { Order = order, Lines = items.Items, ShippingRates = shippingSelection };
		}

		private IEnumerable<string> ListShipmentsWithoutSelection(MarketplaceOrder order, IList<LineItem> items)
		{
			var shipFromAddressIDs = items.GroupBy(li => li.ShipFromAddressID).Select(group => group.Key);
			var selections = order.xp.ProposedShipmentSelections.Select(sel => sel.ShipFromAddressID);
			return shipFromAddressIDs.Except(selections);
		}
	}
}
