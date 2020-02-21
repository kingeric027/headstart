using Marketplace.Common.Models;
using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;
using Marketplace.Common.Services;

namespace Marketplace.Common.Commands
{
    public interface IAddressValidationCommand
    {
        Task<WebhookResponse> IsValidAddressInFreightPopAsync(Address address);
        Task<WebhookResponse> GetExpectedNewSellerAddressAndValidateInFreightPop(WebhookPayloads.AdminAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewSupplierAddressAndValidateInFreightPop(WebhookPayloads.SupplierAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewMeAddressAndValidateInFreightPop(WebhookPayloads.Me.PatchAddress payload);
        Task<WebhookResponse> GetExpectedNewBuyerAddressAndValidateInFreightPop(WebhookPayloads.Addresses.Patch payload);
    }
    public class AddressValidationCommand : IAddressValidationCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;
		private readonly ISmartyStreetsService _smartyStreets;
		private readonly WebhookResponse validResponse = new WebhookResponse { proceed = true };
		private readonly AddressValidationPreWebhookError inValidResponse; 
        public AddressValidationCommand(IFreightPopService freightPopService, IOCShippingIntegration ocShippingIntegration, IOrderCloudClient ocClient, ISmartyStreetsService smartyStreets)
        {
            _freightPopService = freightPopService;
            _oc = ocClient;
            _ocShippingIntegration = ocShippingIntegration;
			_smartyStreets = smartyStreets;
			inValidResponse = new AddressValidationPreWebhookError(_smartyStreets.GetSuggestedAddresses(null));
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
