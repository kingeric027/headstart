using Marketplace.Common.Models;
using Marketplace.Common.Services.ShippingIntegration;
using OrderCloud.SDK;
using System;
using System.Threading.Tasks;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Common.Services.ShippingIntegration.Mappers;

namespace Marketplace.Common.Commands
{
    public interface IAddressValidationCommand
    {
        Task<PrewebhookResponseWithError> IsValidAddressInFreightPopAsync(Address address);
        Task<PrewebhookResponseWithError> GetExpectedNewSellerAddressAndValidateInFreightPop(WebhookPayloads.AdminAddresses.Patch payload);
        Task<PrewebhookResponseWithError> GetExpectedNewSupplierAddressAndValidateInFreightPop(WebhookPayloads.SupplierAddresses.Patch payload);
        Task<PrewebhookResponseWithError> GetExpectedNewMeAddressAndValidateInFreightPop(WebhookPayloads.Me.PatchAddress payload);
        Task<PrewebhookResponseWithError> GetExpectedNewBuyerAddressAndValidateInFreightPop(WebhookPayloads.Addresses.Patch payload);
    }
    public class AddressValidationCommand : IAddressValidationCommand
    {
        private readonly IFreightPopService _freightPopService;
        private readonly IOrderCloudClient _oc;
        private readonly IOCShippingIntegration _ocShippingIntegration;
        private readonly PrewebhookResponseWithError validResponse = new PrewebhookResponseWithError { proceed = true };
        private readonly PrewebhookResponseWithError inValidResponse = new PrewebhookResponseWithError { proceed = false, body = "Address invalid, please try again" };
        public AddressValidationCommand(IFreightPopService freightPopService, IOCShippingIntegration ocShippingIntegration, IOrderCloudClient ocClient)
        {
            _freightPopService = freightPopService;
            _oc = ocClient;
            _ocShippingIntegration = ocShippingIntegration;
        }

        public async Task<PrewebhookResponseWithError> IsValidAddressInFreightPopAsync(Address address)
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

        public async Task<PrewebhookResponseWithError> IsValidAddressInFreightPopAsync(BuyerAddress address)
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

        public async Task<PrewebhookResponseWithError> GetExpectedNewSellerAddressAndValidateInFreightPop(WebhookPayloads.AdminAddresses.Patch payload)
        {
            var existingAddress = await _oc.AdminAddresses.GetAsync(payload.RouteParams.AddressID);
            var expectedNewAddress = GetExpectedNewAddress(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressInFreightPopAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<PrewebhookResponseWithError> GetExpectedNewSupplierAddressAndValidateInFreightPop(WebhookPayloads.SupplierAddresses.Patch payload)
        {
            var existingAddress = await _oc.SupplierAddresses.GetAsync(payload.RouteParams.SupplierID, payload.RouteParams.AddressID);
            var expectedNewAddress = GetExpectedNewAddress(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressInFreightPopAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<PrewebhookResponseWithError> GetExpectedNewMeAddressAndValidateInFreightPop(WebhookPayloads.Me.PatchAddress payload)
        {
            var userToken = payload.UserToken;
            var existingAddress = await _oc.Me.GetAddressAsync(payload.RouteParams.AddressID, userToken);
            var expectedNewAddress = GetExpectedNewAddress(payload.Request.Body, existingAddress);
            var ratesResponse = await IsValidAddressInFreightPopAsync(expectedNewAddress);
            return ratesResponse;
        }

        public async Task<PrewebhookResponseWithError> GetExpectedNewBuyerAddressAndValidateInFreightPop(WebhookPayloads.Addresses.Patch payload)
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
