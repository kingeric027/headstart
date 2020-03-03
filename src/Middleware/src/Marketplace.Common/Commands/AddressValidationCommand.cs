using Marketplace.Common.Models;
using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System;
using System.Threading.Tasks;
using Marketplace.Common.Services;

namespace Marketplace.Common.Commands
{
    public interface IAddressValidationCommand
    {
        Task<WebhookResponse> IsValidAddressAsync(Address address);
        Task<WebhookResponse> GetExpectedNewSellerAddressAndValidateInFreightPop(WebhookPayloads.AdminAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewSupplierAddressAndValidateInFreightPop(WebhookPayloads.SupplierAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewMeAddressAndValidateInFreightPop(WebhookPayloads.Me.PatchAddress payload);
        Task<WebhookResponse> GetExpectedNewBuyerAddressAndValidateInFreightPop(WebhookPayloads.Addresses.Patch payload);
    }
    public class AddressValidationCommand : IAddressValidationCommand
    {
        private readonly IOrderCloudClient _oc;
		private readonly ISmartyStreetsService _smartyStreets;
		public AddressValidationCommand(IOrderCloudClient ocClient, ISmartyStreetsService smartyStreets)
        {
            _oc = ocClient;
			_smartyStreets = smartyStreets;
		}

        public async Task<WebhookResponse> IsValidAddressAsync(Address address)
        {
            try
            {
				var validation = await _smartyStreets.ValidateAddress(address);
				return new AddressValidationWebhookResponse(validation);
			} catch (Exception ex)
            {
				return new WebhookResponse<Exception>(ex);
			}
        }

		public async Task<WebhookResponse> IsValidAddressAsync(BuyerAddress address)
        {
            try
            {
				var validation = await _smartyStreets.ValidateAddress(address);
				return new AddressValidationWebhookResponse(validation);
			}
            catch (Exception ex)
            {
				return new WebhookResponse<Exception>(ex);
			}
		}

        public async Task<WebhookResponse> GetExpectedNewSellerAddressAndValidateInFreightPop(WebhookPayloads.AdminAddresses.Patch payload)
        {
            var existingAddress = await _oc.AdminAddresses.GetAsync(payload.RouteParams.AddressID);
            var expectedNewAddress = PatchObject(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewSupplierAddressAndValidateInFreightPop(WebhookPayloads.SupplierAddresses.Patch payload)
        {
            var existingAddress = await _oc.SupplierAddresses.GetAsync(payload.RouteParams.SupplierID, payload.RouteParams.AddressID);
            var expectedNewAddress = PatchObject(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewMeAddressAndValidateInFreightPop(WebhookPayloads.Me.PatchAddress payload)
        {
            var userToken = payload.UserToken;
            var existingAddress = await _oc.Me.GetAddressAsync(payload.RouteParams.AddressID, userToken);
            var expectedNewAddress = PatchObject(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewBuyerAddressAndValidateInFreightPop(WebhookPayloads.Addresses.Patch payload)
        {
            var existingAddress = await _oc.Addresses.GetAsync(payload.RouteParams.BuyerID, payload.RouteParams.AddressID);
            var expectedNewAddress = PatchObject(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressAsync(expectedNewAddress);
            return ratesResponse;
        }

		// todo: move somewhere else, see if we can use func in helper lib
		private T PatchObject<T>(T patch, T existing)
		{
			// todo: add test for this function 
			var patchType = patch.GetType();
			var propertiesInPatch = patchType.GetProperties();
			foreach (var property in propertiesInPatch)
			{
				var patchValue = property.GetValue(patch);
				if (patchValue != null)
				{
					property.SetValue(existing, patchValue, null);
				}
			}
			return existing;
		}
	}
}
