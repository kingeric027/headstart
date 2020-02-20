using Marketplace.Common.Helpers;
using Marketplace.Common.Models;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models;
using Marketplace.Models.Exceptions;
using Marketplace.Models.Extended;
using Marketplace.Common.Services;

namespace Marketplace.Common.Commands
{
    public interface IProposedShipmentCommand
    {
        Task<ListPage<ProposedShipment>> ListProposedShipments(string orderId, VerifiedUserContext userContext);
        Task<MarketplaceOrder> SetShippingSelectionAsync(string orderID, ProposedShipmentSelection selection);
        Task<WebhookResponse> IsValidAddressInFreightPopAsync(Address address);
        Task<WebhookResponse> GetExpectedNewSellerAddressAndValidateInFreightPop(WebhookPayloads.AdminAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewSupplierAddressAndValidateInFreightPop(WebhookPayloads.SupplierAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewMeAddressAndValidateInFreightPop(WebhookPayloads.Me.PatchAddress payload);
        Task<WebhookResponse> GetExpectedNewBuyerAddressAndValidateInFreightPop(WebhookPayloads.Addresses.Patch payload);
    }
    public class ProposedShipmentCommand : IProposedShipmentCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;
		private readonly ISmartyStreetsService _smartyStreets;
		private readonly WebhookResponse validResponse = new WebhookResponse { proceed = true };
		private readonly AddressValidationPreWebhookError inValidResponse; 
        public ProposedShipmentCommand(IFreightPopService freightPopService, IOCShippingIntegration ocShippingIntegration, IOrderCloudClient ocClient, ISmartyStreetsService smartyStreets)
        {
            _freightPopService = freightPopService;
            _oc = ocClient;
            _ocShippingIntegration = ocShippingIntegration;
			_smartyStreets = smartyStreets;
			inValidResponse = new AddressValidationPreWebhookError(_smartyStreets.GetSuggestedAddresses(null));
		}

        public async Task<ListPage<ProposedShipment>> ListProposedShipments(string orderId, VerifiedUserContext userContext)
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

            return new ListPage<ProposedShipment>()
            {
                Items = proposedShipments,
                Meta = new ListPageMeta
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
            Require.That(exists, Marketplace.Models.ErrorCodes.Checkout.InvalidShipFromAddress, new InvalidShipFromAddressIDError(selection.ShipFromAddressID));

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

        public async Task<WebhookResponse> IsValidAddressInFreightPopAsync(Address address)
        {
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

        public async Task<WebhookResponse> IsValidAddressInFreightPopAsync(BuyerAddress address)
        {
            var rateRequestBody = AddressValidationRateRequestMapper.Map(address);
            try
            {
                var ratesResponse = await _freightPopService.GetRatesAsync(rateRequestBody);
                return ratesResponse.Data.ErrorMessages.Count > 0 ? inValidResponse : validResponse;
            }
            catch (Exception ex)
            {
                return inValidResponse;
            }
        }

        public async Task<WebhookResponse> GetExpectedNewSellerAddressAndValidateInFreightPop(WebhookPayloads.AdminAddresses.Patch payload)
        {
            var existingAddress = await _oc.AdminAddresses.GetAsync(payload.RouteParams.AddressID);
            var expectedNewAddress = GetExpectedNewAddress(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressInFreightPopAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewSupplierAddressAndValidateInFreightPop(WebhookPayloads.SupplierAddresses.Patch payload)
        {
            var existingAddress = await _oc.SupplierAddresses.GetAsync(payload.RouteParams.SupplierID, payload.RouteParams.AddressID);
            var expectedNewAddress = GetExpectedNewAddress(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressInFreightPopAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewMeAddressAndValidateInFreightPop(WebhookPayloads.Me.PatchAddress payload)
        {
            var userToken = payload.UserToken;
            var existingAddress = await _oc.Me.GetAddressAsync(payload.RouteParams.AddressID, userToken);
            var expectedNewAddress = GetExpectedNewAddress(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressInFreightPopAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewBuyerAddressAndValidateInFreightPop(WebhookPayloads.Addresses.Patch payload)
        {
            var existingAddress = await _oc.Addresses.GetAsync(payload.RouteParams.BuyerID, payload.RouteParams.AddressID);
            var expectedNewAddress = GetExpectedNewAddress(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressInFreightPopAsync(expectedNewAddress);
            return ratesResponse;
        }

        private BuyerAddress GetExpectedNewAddress(BuyerAddress patch, BuyerAddress existingAddress)
        {

            // todo: add test for this function 

            var patchType = patch.GetType();
            var propertiesInPatch = patchType.GetProperties();
            foreach (var property in propertiesInPatch)
            {
                var patchValue = property.GetValue(patch);
                if (patchValue != null)
                {
                    property.SetValue(existingAddress, patchValue, null);
                }
            }
            return existingAddress;
        }

        private Address GetExpectedNewAddress(Address patch, Address existingAddress)
        {

            // todo: add test for this function 

            var patchType = patch.GetType();
            var propertiesInPatch = patchType.GetProperties();
            foreach (var property in propertiesInPatch)
            {
                var patchValue = property.GetValue(patch);
                if (patchValue != null)
                {
                    property.SetValue(existingAddress, patchValue, null);
                }
            }
            return existingAddress;
        }
    }
}
