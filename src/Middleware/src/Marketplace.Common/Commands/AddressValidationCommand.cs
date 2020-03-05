using Marketplace.Common.Models;
using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System;
using System.Threading.Tasks;
using Marketplace.Common.Services;
using Marketplace.Models;

namespace Marketplace.Common.Commands
{
    public interface IAddressValidationCommand
    {
        Task<WebhookResponse> IsValidAddressAsync(Address address);
        Task<WebhookResponse> GetExpectedNewSellerAddressAndValidate(WebhookPayloads.AdminAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewSupplierAddressAndValidate(WebhookPayloads.SupplierAddresses.Patch payload);
        Task<WebhookResponse> GetExpectedNewMeAddressAndValidate(WebhookPayloads.Me.PatchAddress payload);
        Task<WebhookResponse> GetExpectedNewBuyerAddressAndValidate(WebhookPayloads.Addresses.Patch payload);
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

        public async Task<WebhookResponse> GetExpectedNewSellerAddressAndValidate(WebhookPayloads.AdminAddresses.Patch payload)
        {
            var existingAddress = await _oc.AdminAddresses.GetAsync<Address>(payload.RouteParams.AddressID);
            var expectedNewAddress = PatchObject(payload.Request.Body as Address, existingAddress);
            var ratesResponse = await IsValidAddressAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewSupplierAddressAndValidate(WebhookPayloads.SupplierAddresses.Patch payload)
        {
            var existingAddress = await _oc.SupplierAddresses.GetAsync<Address>(payload.RouteParams.SupplierID, payload.RouteParams.AddressID);
            var expectedNewAddress = PatchObject(payload.Request.Body as Address, existingAddress);
            var ratesResponse = await IsValidAddressAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewMeAddressAndValidate(WebhookPayloads.Me.PatchAddress payload)
        {
            var userToken = payload.UserToken;
            var existingAddress = await _oc.Me.GetAddressAsync<BuyerAddress>(payload.RouteParams.AddressID, userToken);
            var expectedNewAddress = PatchObject(payload.Request.Body as BuyerAddress, existingAddress);
            var ratesResponse = await IsValidAddressAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<WebhookResponse> GetExpectedNewBuyerAddressAndValidate(WebhookPayloads.Addresses.Patch payload)
        {
            var existingAddress = await _oc.Addresses.GetAsync<Address>(payload.RouteParams.BuyerID, payload.RouteParams.AddressID);
            var expectedNewAddress = PatchObject(payload.Request.Body as Address, existingAddress);
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
