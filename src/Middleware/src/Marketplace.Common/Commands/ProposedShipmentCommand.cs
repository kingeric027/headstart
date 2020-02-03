using Marketplace.Common.Exceptions;
using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Services;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IProposedShipmentCommand
    {
        Task<MarketplaceListPage<ProposedShipment>> ListProposedShipments(string orderId, VerifiedUserContext userContext);
        Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, ProposedShipmentSelection selection);
        Task<PrewebhookResponseWithError> IsValidAddressInFreightPopAsync(Address address);
    }
    public class ProposedShipmentCommand : IProposedShipmentCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;
        public ProposedShipmentCommand(IFreightPopService freightPopService, IOCShippingIntegration ocShippingIntegration)
        {
            _freightPopService = freightPopService;
            _oc = OcFactory.GetSEBAdmin();
            _ocShippingIntegration = ocShippingIntegration;
        }

        public async Task<MarketplaceListPage<ProposedShipment>> ListProposedShipments(string orderId, VerifiedUserContext userContext)
        {
            var order = await _oc.Orders.GetAsync(OrderDirection.Outgoing, orderId, userContext.AccessToken);

            // update to accomodate lots of lineitems
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Outgoing, orderId, accessToken: userContext.AccessToken);
            var superOrder = new SuperOrder
            {
                Order = order,
                LineItems = lineItems.Items.ToList(),
            };

            var proposedShipments = await _ocShippingIntegration.GetProposedShipmentsForSuperOrderAsync(superOrder);

            return new MarketplaceListPage<ProposedShipment>()
            {
                Items = proposedShipments,
                Meta = new MarketplaceListPageMeta
                {
                    Page = 1,
                    PageSize = 100,
                    TotalCount = proposedShipments.Count(),
                }
            };
        }

        public async Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, ProposedShipmentSelection selection)
        {
            var order = await _oc.Orders.GetAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID);
            var lineItems = await _oc.LineItems.ListAsync(OrderDirection.Incoming, orderID);

            var exists = lineItems.Items.Any(li => li.ShipFromAddressID == selection.ShipFromAddressID);
            Require.That(exists, Exceptions.ErrorCodes.Checkout.InvalidShipFromAddress, new InvalidShipFromAddressIDError(selection.ShipFromAddressID));

            var selections = order.xp?.ProposedShipmentSelections?.ToDictionary(s => s.ShipFromAddressID) ?? new Dictionary<string, ProposedShipmentSelection> { };
            selections[selection.ShipFromAddressID] = selection;
            var totalShippingCost = selections
                .Sum(sel => sel.Value.Rate);

                return await _oc.Orders.PatchAsync<MarketplaceOrder>(OrderDirection.Incoming, orderID, new PartialOrder()
            {
                ShippingCost = totalShippingCost,
                xp = new
                {
                    ProposedShipmentSelections = selections.Values.ToArray()
                }
            });
        }

        public async Task<PrewebhookResponseWithError> IsValidAddressInFreightPopAsync(Address address)
        {
            var validResponse = new PrewebhookResponseWithError { proceed = true };
            var inValidResponse = new PrewebhookResponseWithError { proceed = false, body = "Address invalid, please try again" };
            var rateRequestBody = AddressValidationRateRequestMapper.Map(address);
            try
            {
                var ratesResponse = await _freightPopService.GetRatesAsync(rateRequestBody);
                if(ratesResponse.Data.ErrorMessages.Count > 0)
                {
                    return inValidResponse;
                } else
                {
                    return validResponse;
                }
            } catch (Exception ex)
            {
                return inValidResponse;
            }
        }
    }
}
