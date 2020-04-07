using Marketplace.Common.Commands;
using Marketplace.Helpers.Attributes;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Marketplace.Common.Controllers
{
	[DocComments("\"Integration\" represents Validated Addresses with suggestions")]
	[MarketplaceSection.Integration(ListOrder = 5)]
	public class ValidatedAddressController: BaseController
	{
		private readonly IOrderCloudClient _oc;
		private readonly IValidatedAddressCommand _addressCommand;
		public ValidatedAddressController(AppSettings settings) : base(settings)
		{
			
		}

		// ME endpoints
		[HttpPost, Route("me/addresses"), MarketplaceUserAuth(ApiRole.MeAddressAdmin)]
		public async Task<BuyerAddress> CreateMeAddress([FromBody] BuyerAddress address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Me.CreateAddressAsync(validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("me/addresses/{addressID}"), MarketplaceUserAuth(ApiRole.MeAddressAdmin)]
		public async Task<BuyerAddress> SaveMeAddress(string addressID, [FromBody] BuyerAddress address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Me.SaveAddressAsync(addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("me/addresses/{addressID}"), MarketplaceUserAuth(ApiRole.MeAddressAdmin)]
		public async Task PatchMeAddress(string addressID, [FromBody] PartialBuyerAddress patch)
		{
			var address = await _addressCommand.GetPatchedMeAddress(addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(address);
			await _oc.Me.PatchAddressAsync(addressID, patch, VerifiedUserContext.AccessToken);
		}

		// BUYER endpoints

		[HttpPost, Route("buyers/{buyerID}/addresses"), MarketplaceUserAuth(ApiRole.AddressAdmin)]
		public async Task<Address> CreateBuyerAddress(string buyerID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Addresses.CreateAsync(buyerID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("buyers/{buyerID}/addresses/{addressID}"), MarketplaceUserAuth(ApiRole.AddressAdmin)]
		public async Task<Address> SaveBuyerAddress(string buyerID, string addressID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Addresses.SaveAsync(buyerID, addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("buyers/{buyerID}/addresses/{addressID}"), MarketplaceUserAuth(ApiRole.AddressAdmin)]
		public async Task<Address> PatchBuyerAddress(string buyerID, string addressID, [FromBody] PartialAddress patch)
		{
			var address = await _addressCommand.GetPatchedBuyerAddress(buyerID, addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(address);
			return await _oc.Addresses.PatchAsync(buyerID, addressID, patch, VerifiedUserContext.AccessToken);
		}

		// SUPPLIER endpoints

		[HttpPost, Route("suppliers/{supplierID}/addresses"), MarketplaceUserAuth(ApiRole.SupplierAddressAdmin)]
		public async Task<Address> CreateSupplierAddress(string supplierID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.SupplierAddresses.CreateAsync(supplierID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("suppliers/{supplierID}/addresses/{addressID}"), MarketplaceUserAuth(ApiRole.SupplierAddressAdmin)]
		public async Task<Address> SaveSupplierAddress(string supplierID, string addressID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.SupplierAddresses.SaveAsync(supplierID, addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("suppliers/{supplierID}/addresses/{addressID}"), MarketplaceUserAuth(ApiRole.SupplierAddressAdmin)]
		public async Task<Address> PatchSupplierAddress(string supplierID, string addressID, [FromBody] PartialAddress patch)
		{
			var address = await _addressCommand.GetPatchedSupplierAddress(supplierID, addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(address);
			return await _oc.SupplierAddresses.PatchAsync(supplierID, addressID, patch, VerifiedUserContext.AccessToken);
		}

		// ADMIN endpoints

		[HttpPost, Route("addresses"), MarketplaceUserAuth(ApiRole.AdminAddressAdmin)]
		public async Task<Address> CreateAdminAddress([FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.AdminAddresses.CreateAsync(address, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("addresses/{addressID}"), MarketplaceUserAuth(ApiRole.AdminAddressAdmin)]
		public async Task<Address> SaveAdminAddress(string addressID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.AdminAddresses.SaveAsync(addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("addresses/{addressID}"), MarketplaceUserAuth(ApiRole.AdminAddressAdmin)]
		public async Task<Address> PatchAdminAddress(string addressID, [FromBody] PartialAddress patch)
		{
			var address = _addressCommand.GetPatchedAdminAddress(addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(patch);
			return await _oc.AdminAddresses.PatchAsync(addressID, patch, VerifiedUserContext.AccessToken);
		}

		// ORDER endpoints

		[HttpPut, Route("order/{direction}/{orderID}/billto"), MarketplaceUserAuth(ApiRole.Shopper, ApiRole.OrderAdmin)]
		public async Task<Order> SetBillingAddress(OrderDirection direction, string orderID, [FromBody] PartialAddress address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Orders.SetBillingAddressAsync(direction, orderID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("order/{direction}/{orderID}/shipto"), MarketplaceUserAuth(ApiRole.Shopper, ApiRole.OrderAdmin)]
		public async Task<Order> SetShippingAddress(OrderDirection direction, string orderID, [FromBody] PartialAddress address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Orders.SetShippingAddressAsync(direction, orderID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}
	}
}
