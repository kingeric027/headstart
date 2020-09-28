using Marketplace.Common.Commands;
using Marketplace.Models;
using Marketplace.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;

namespace Marketplace.Common.Controllers
{
    [DocComments("\"Files\" represents files for Marketplace content management control")]
    [MarketplaceSection.Marketplace(ListOrder = 6)]
    [Route("buyerlocations")]
    public class BuyerLocationController : BaseController
    {
        private readonly IMarketplaceBuyerLocationCommand _buyerLocationCommand;
        private readonly IOrderCloudClient _oc;
        private readonly ILocationPermissionCommand _locationPermissionCommand;
        public BuyerLocationController(ILocationPermissionCommand locationPermissionCommand, IMarketplaceBuyerLocationCommand buyerLocationCommand, IOrderCloudClient oc, AppSettings settings) : base(settings)
        {
            _buyerLocationCommand = buyerLocationCommand;
            _locationPermissionCommand = locationPermissionCommand;
            _oc = oc;
        }

        [DocName("GET a Buyer Location")]
        [HttpGet, Route("{buyerID}/{buyerLocationID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID)
        {
            return await _buyerLocationCommand.Get(buyerID, buyerLocationID, VerifiedUserContext);
        }

        [DocName("POST a Buyer Location")]
        [HttpPost, Route("{buyerID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Create(string buyerID, [FromBody] MarketplaceBuyerLocation buyerLocation)
        {
            // ocAuth is the token for the organization that is specified in the AppSettings
            var ocAuth = await _oc.AuthenticateAsync();
            return await _buyerLocationCommand.Create(buyerID, buyerLocation, VerifiedUserContext, ocAuth.AccessToken);
        }

        [DocName("PUT a Buyer Location")]
        [HttpPut, Route("{buyerID}/{buyerLocationID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task<MarketplaceBuyerLocation> Save(string buyerID, string buyerLocationID, [FromBody] MarketplaceBuyerLocation buyerLocation)
        {
            return await _buyerLocationCommand.Save(buyerID, buyerLocationID, buyerLocation, VerifiedUserContext);
        }

        [DocName("Delete a Buyer Location")]
        [HttpDelete, Route("{buyerID}/{buyerLocationID}"), OrderCloudIntegrationsAuth(ApiRole.UserGroupAdmin, ApiRole.AddressAdmin)]
        public async Task Delete(string buyerID, string buyerLocationID)
        {
            await _buyerLocationCommand.Delete(buyerID, buyerLocationID, VerifiedUserContext);
        }

        [DocName("GET List of location permission user groups")]
        [HttpGet]
        [OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        [Route("{buyerID}/{buyerLocationID}/permissions")]
        public async Task<List<UserGroupAssignment>> ListLocationPermissionUserGroups(string buyerID, string buyerLocationID)
        {
            return await _locationPermissionCommand.ListLocationPermissionAsssignments(buyerID, buyerLocationID, VerifiedUserContext);
        }

        [DocName("LIST orders for a specific location as a buyer, ensures user has access to location orders")]
        [HttpGet, Route("{buyerID}/{buyerLocationID}/users"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<ListPage<MarketplaceUser>> ListLocationUsers(string buyerID, string buyerLocationID, ListArgs<MarketplaceOrder> listArgs)
        {
            return await _locationPermissionCommand.ListLocationUsers(buyerID, buyerLocationID, VerifiedUserContext);
        }

        [DocName("POST location permissions, add or delete access")]
        [HttpPost, Route("{buyerID}/{buyerLocationID}/permissions"), OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        public async Task<List<UserGroupAssignment>> UpdateLocationPermissions(string buyerID, string buyerLocationID, [FromBody] LocationPermissionUpdate locationPermissionUpdate)
        {
            return await _locationPermissionCommand.UpdateLocationPermissions(buyerID, buyerLocationID, locationPermissionUpdate, VerifiedUserContext);
        }

        [DocName("GET List of location approval permission user groups")]
        [HttpGet]
        [OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        [Route("{buyerID}/{buyerLocationID}/approvalpermissions")]
        public async Task<List<UserGroupAssignment>> ListLocationApprovalPermissionAsssignments(string buyerID, string buyerLocationID)
        {
            return await _locationPermissionCommand.ListLocationPermissionAsssignments(buyerID, buyerLocationID, VerifiedUserContext);
        }

        [DocName("GET general approval threshold for location")]
        [HttpGet]
        [OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        [Route("{buyerID}/{buyerLocationID}/approvalthreshold")]
        public async Task<decimal> GetApprovalThreshold(string buyerID, string buyerLocationID)
        {
            return await _locationPermissionCommand.GetApprovalThreshold(buyerID, buyerLocationID, VerifiedUserContext);
        }

        [DocName("POST set location approval threshold")]
        [HttpPost]
        [OrderCloudIntegrationsAuth(ApiRole.Shopper)]
        [Route("{buyerID}/{buyerLocationID}/approvalthreshold")]
        public async Task<decimal> SetLocationApprovalThreshold(string buyerID, string buyerLocationID, [FromBody] LocationApprovalThresholdUpdate locationApprovalThresholdUpdate)
        {
            return await _locationPermissionCommand.SetLocationApprovalThreshold(buyerID, buyerLocationID, locationApprovalThresholdUpdate.Threshold, VerifiedUserContext);
        }
    }
}
