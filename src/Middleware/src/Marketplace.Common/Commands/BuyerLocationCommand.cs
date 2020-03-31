using Marketplace.Common.Helpers;
using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Common.TemporaryAppConstants;
using Marketplace.Helpers;using Marketplace.Helpers.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Marketplace;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceBuyerLocationCommand
    {
        Task<MarketplaceBuyerLocation> Create(string buyerID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user, string token);
        Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID, VerifiedUserContext user);
        Task<MarketplaceBuyerLocation> Update(string buyerID, string buyerLocationID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user);
        Task Delete(string buyerID, string buyerLocationID, VerifiedUserContext user);	}

    public class MarketplaceBuyerLocationCommand : IMarketplaceBuyerLocationCommand
    {
        private readonly IOrderCloudClient _oc;
		private readonly AppSettings _settings;
        public MarketplaceBuyerLocationCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }
        public async Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID, VerifiedUserContext user)        {            var buyerAddress = await _oc.Addresses.GetAsync<MarketplaceAddressBuyer>(buyerID, buyerLocationID, accessToken: user.AccessToken);            var buyerUserGroup = await _oc.UserGroups.GetAsync<MarketplaceUserGroup>(buyerID, buyerLocationID, accessToken: user.AccessToken);            return new MarketplaceBuyerLocation            {                Address = buyerAddress,                UserGroup = buyerUserGroup            };        }
        public async Task<MarketplaceBuyerLocation> Create(string buyerID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user, string token)
        {
            var buyerLocationID = CreateBuyerLocationID(buyerID, buyerLocation);            buyerLocation.Address.ID = buyerLocationID;            var buyerAddress = await _oc.Addresses.CreateAsync<MarketplaceAddressBuyer>(buyerID, buyerLocation.Address, accessToken: user.AccessToken);
            buyerLocation.UserGroup.ID = buyerAddress.ID;
            var buyerUserGroup = await _oc.UserGroups.CreateAsync<MarketplaceUserGroup>(buyerID, buyerLocation.UserGroup, accessToken: user.AccessToken);
            await CreateUserGroupAndAssignments(token, buyerID, buyerUserGroup.ID, buyerAddress.ID);
            await CreateApprovalUserGroupsAndApprovalRule(token, buyerAddress.ID);
            return new MarketplaceBuyerLocation            {                Address = buyerAddress,                UserGroup = buyerUserGroup,            };
        }

        private string CreateBuyerLocationID(string buyerID, MarketplaceBuyerLocation buyerLocation)        {            var addressIDInRequest = buyerLocation.Address.ID;            if(addressIDInRequest.Contains("LocationIncrementor"))            {                // prevents prefix duplication with address validation prewebhooks                return addressIDInRequest;            }            if (addressIDInRequest == null || addressIDInRequest.Length == 0)            {                return buyerID + "-{" + buyerID + "-LocationIncrementor}";            } else            {                return buyerID + "-" + addressIDInRequest;            }        }

        public async Task CreateUserGroupAndAssignments(string token, string buyerID, string buyerUserGroupID, string buyerAddressID)
        {
            var assignment = new AddressAssignment            {                AddressID = buyerAddressID,                UserGroupID = buyerUserGroupID,                IsBilling = true,                IsShipping = true            };
            await _oc.Addresses.SaveAssignmentAsync(buyerID, assignment, accessToken: token);
        }

        public async Task<MarketplaceBuyerLocation> Update(string buyerID, string buyerLocationID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user)
        {
            var updatedBuyerAddress = await _oc.Addresses.SaveAsync<MarketplaceAddressBuyer>(buyerID, buyerLocation.Address.ID, buyerLocation.Address, accessToken: user.AccessToken);
            var updatedBuyerUserGroup = await _oc.UserGroups.SaveAsync<MarketplaceUserGroup>(buyerID, buyerLocation.UserGroup.ID, buyerLocation.UserGroup, accessToken: user.AccessToken);
            return new MarketplaceBuyerLocation            {                Address = updatedBuyerAddress,                UserGroup = updatedBuyerUserGroup,            };
        }
        
        public async Task Delete(string buyerID, string buyerLocationID, VerifiedUserContext user)
        {
            var deleteAddressReq = _oc.Addresses.DeleteAsync(buyerID, buyerLocationID, accessToken: user.AccessToken);
            var deleteUserGroupReq = _oc.UserGroups.DeleteAsync(buyerID, buyerLocationID, accessToken: user.AccessToken);
            await Task.WhenAll(deleteAddressReq, deleteUserGroupReq);
        }
        public async Task CreateApprovalUserGroupsAndApprovalRule(string token, string buyerLocationID)
        {            var buyerID = buyerLocationID.Split('-').First();            var approvingGroupID = $"{buyerLocationID}-OrderApprover";            foreach (var userType in SEBUserTypes.BuyerLocation())            {                var userGroupID = $"{buyerLocationID}-{userType.UserGroupIDSuffix}";                await _oc.UserGroups.CreateAsync(buyerID, new UserGroup()                {                    ID = userGroupID,                    Name = userType.UserGroupName,                    xp =                        {                            Type = "Approval",                            Location = buyerLocationID                        }                }, token);            }            await _oc.ApprovalRules.CreateAsync(buyerID, new ApprovalRule()            {                ID = buyerLocationID,                ApprovingGroupID = approvingGroupID,                Description = "General Approval Rule for Location. Every Order Over a Certain Limit will Require Approval for the designated group of users.",                Name = "General Location Approval Rule",                RuleExpression = $"order.xp.ApprovalNeeded = {buyerLocationID} && order.Total > 0"            });        }
    }
}
