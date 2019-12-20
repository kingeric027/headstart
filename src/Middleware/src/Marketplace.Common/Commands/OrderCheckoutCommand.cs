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
		Task<Order> SetShippingAndTax(string orderID, string shippingQuoteID);
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

		public async Task<Order> SetShippingAndTax(string orderID, string shippingQuoteID)
		{	
			var order = await _oc.Orders.GetAsync(OrderDirection.Outgoing, orderID);
			var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderID);

			var shippingQuote = await _shipping.GetSavedShippingQuote(orderID, shippingQuoteID);
			var taxTransaction = await _avatax.CreateTaxTransactionAsync(order, lineItems);

			var patchOrder = new PartialOrder()
			{
				ShippingCost = shippingQuote.Cost,
				TaxCost = taxTransaction.totalTax ?? 0,
				xp = new {
					ShippingQuoteID = shippingQuote.ID,     // TODO - These xp's are placeholders until we actually define them.
					AvalaraTaxTransactionCode = taxTransaction.code
				}
			};

			return await _oc.Orders.PatchAsync(OrderDirection.Outgoing, orderID, patchOrder);
		}
	}
}
