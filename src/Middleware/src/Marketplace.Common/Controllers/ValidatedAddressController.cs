using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using ordercloud.integrations.extensions;
using ordercloud.integrations.openapispec;

namespace Marketplace.Common.Controllers
{
	[DocComments("\"Integration\" represents Validated Addresses with suggestions")]
	[MarketplaceSection.Integration(ListOrder = 5)]
	public class ValidatedAddressController: BaseController
	{
		private readonly IOrderCloudClient _oc;
		private readonly IValidatedAddressCommand _addressCommand;
		public ValidatedAddressController(IOrderCloudClient oc, IValidatedAddressCommand command, AppSettings settings) : base(settings)
		{
			_oc = oc;
			_addressCommand = command;
		}

		// ME endpoints
		[HttpPost, Route("me/addresses"), OrderCloudIntegrationsAuth(ApiRole.MeAddressAdmin)]
		public async Task<BuyerAddress> CreateMeAddress([FromBody] BuyerAddress address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Me.CreateAddressAsync(validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("me/addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.MeAddressAdmin)]
		public async Task<BuyerAddress> SaveMeAddress(string addressID, [FromBody] BuyerAddress address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Me.SaveAddressAsync(addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("me/addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.MeAddressAdmin)]
		public async Task PatchMeAddress(string addressID, [FromBody] BuyerAddress patch)
		{
			var address = await _addressCommand.GetPatchedMeAddress(addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(address);
			await _oc.Me.PatchAddressAsync(addressID, (PartialBuyerAddress)patch, VerifiedUserContext.AccessToken);
		}

		// BUYER endpoints

		[HttpPost, Route("buyers/{buyerID}/addresses"), OrderCloudIntegrationsAuth(ApiRole.AddressAdmin)]
		public async Task<Address> CreateBuyerAddress(string buyerID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Addresses.CreateAsync(buyerID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("buyers/{buyerID}/addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.AddressAdmin)]
		public async Task<Address> SaveBuyerAddress(string buyerID, string addressID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Addresses.SaveAsync(buyerID, addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("buyers/{buyerID}/addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.AddressAdmin)]
		public async Task<Address> PatchBuyerAddress(string buyerID, string addressID, [FromBody] Address patch)
		{
			var address = await _addressCommand.GetPatchedBuyerAddress(buyerID, addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(address);
			return await _oc.Addresses.PatchAsync(buyerID, addressID, patch as PartialAddress, VerifiedUserContext.AccessToken);
		}

		// SUPPLIER endpoints

		[HttpPost, Route("suppliers/{supplierID}/addresses"), OrderCloudIntegrationsAuth(ApiRole.SupplierAddressAdmin)]
		public async Task<Address> CreateSupplierAddress(string supplierID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.SupplierAddresses.CreateAsync(supplierID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("suppliers/{supplierID}/addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.SupplierAddressAdmin)]
		public async Task<Address> SaveSupplierAddress(string supplierID, string addressID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.SupplierAddresses.SaveAsync(supplierID, addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("suppliers/{supplierID}/addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.SupplierAddressAdmin)]
		public async Task<Address> PatchSupplierAddress(string supplierID, string addressID, [FromBody] Address patch)
		{
			var address = await _addressCommand.GetPatchedSupplierAddress(supplierID, addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(address);
			return await _oc.SupplierAddresses.PatchAsync(supplierID, addressID, patch as PartialAddress, VerifiedUserContext.AccessToken);
		}

		// ADMIN endpoints

		[HttpPost, Route("addresses"), OrderCloudIntegrationsAuth(ApiRole.AdminAddressAdmin)]
		public async Task<Address> CreateAdminAddress([FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.AdminAddresses.CreateAsync(address, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.AdminAddressAdmin)]
		public async Task<Address> SaveAdminAddress(string addressID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.AdminAddresses.SaveAsync(addressID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPatch, Route("addresses/{addressID}"), OrderCloudIntegrationsAuth(ApiRole.AdminAddressAdmin)]
		public async Task<Address> PatchAdminAddress(string addressID, [FromBody] Address patch)
		{
			var address = _addressCommand.GetPatchedAdminAddress(addressID, patch, VerifiedUserContext.AccessToken);
			await _addressCommand.ValidateAddress(patch);
			return await _oc.AdminAddresses.PatchAsync(addressID, patch as PartialAddress, VerifiedUserContext.AccessToken);
		}

		// ORDER endpoints
		[HttpPut, Route("order/{direction}/{orderID}/billto"), OrderCloudIntegrationsAuth(ApiRole.Shopper, ApiRole.OrderAdmin)]
		public async Task<Order> SetBillingAddress(OrderDirection direction, string orderID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Orders.SetBillingAddressAsync(direction, orderID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}

		[HttpPut, Route("order/{direction}/{orderID}/shipto"), OrderCloudIntegrationsAuth(ApiRole.Shopper, ApiRole.OrderAdmin)]
		public async Task<Order> SetShippingAddress(OrderDirection direction, string orderID, [FromBody] Address address)
		{
			var validation = await _addressCommand.ValidateAddress(address);
			return await _oc.Orders.SetShippingAddressAsync(direction, orderID, validation.ValidAddress, VerifiedUserContext.AccessToken);
		}
	}
}
