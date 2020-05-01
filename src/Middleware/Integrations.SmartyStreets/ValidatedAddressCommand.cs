using OrderCloud.SDK;
using System;
using System.Threading.Tasks;
using Marketplace.Common.Services;
using Marketplace.Common.Services.SmartyStreets;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Marketplace.Common.Services.SmartyStreets.Models;

namespace Marketplace.Common.Commands
{
    public interface IValidatedAddressCommand
    {
		Task<AddressValidation<Address>> ValidateAddress(Address address);
		Task<AddressValidation<BuyerAddress>> ValidateAddress(BuyerAddress address);
		Task<Address> GetPatchedAdminAddress(string addressID, Address patch, string token);
		Task<Address> GetPatchedSupplierAddress(string supplierID, string addressID, Address patch, string token);
		Task<Address> GetPatchedBuyerAddress(string buyerID, string addressID, Address patch, string token);
		Task<BuyerAddress> GetPatchedMeAddress(string addressID, BuyerAddress patch, string token);
	}

	public class ValidatedAddressCommand : IValidatedAddressCommand
    {
        private readonly IOrderCloudClient _oc;
		private readonly ISmartyStreetsService _smartyStreets;
		public ValidatedAddressCommand(IOrderCloudClient ocClient, ISmartyStreetsService smartyStreets)
        {
            _oc = ocClient;
			_smartyStreets = smartyStreets;
		}

		public async Task<AddressValidation<Address>> ValidateAddress(Address address)
		{
			var validation = await _smartyStreets.ValidateAddress(address);
			if (!validation.ValidAddressFound) throw new InvalidAddressException(validation);
			return validation;
		}

		public async Task<AddressValidation<BuyerAddress>> ValidateAddress(BuyerAddress address)
		{
			var validation = await _smartyStreets.ValidateAddress(address);
			if (!validation.ValidAddressFound) throw new InvalidBuyerAddressException(validation);
			return validation;
		}

		public async Task<Address> GetPatchedAdminAddress(string addressID, Address patch, string token)
        {
			var current = await _oc.AdminAddresses.GetAsync<Address>(addressID, token);
			var patched = PatchObject(patch, current);
			return patched;
        }

        public async Task<Address> GetPatchedSupplierAddress(string supplierID, string addressID, Address patch, string token)
        {
			var current = await _oc.SupplierAddresses.GetAsync<Address>(supplierID, addressID, token);
			var patched = PatchObject(patch, current);
			return patched;
		}

		public async Task<Address> GetPatchedBuyerAddress(string buyerID, string addressID, Address patch, string token)
		{
			var current = await _oc.Addresses.GetAsync<Address>(buyerID, addressID, token);
			var patched = PatchObject(patch, current);
			return patched;
		}

		public async Task<BuyerAddress> GetPatchedMeAddress(string addressID, BuyerAddress patch, string token)
        {
			var current = await _oc.Me.GetAddressAsync<BuyerAddress>(addressID, token);
			var patched = PatchObject(patch, current);
			return patched;
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
