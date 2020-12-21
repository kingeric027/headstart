﻿using Marketplace.Models;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using System.Linq;
using System.Threading.Tasks;
using ordercloud.integrations.library;
using Marketplace.Common.Constants;
using System;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceBuyerLocationCommand
    {
        Task<MarketplaceBuyerLocation> Create(string buyerID, MarketplaceBuyerLocation buyerLocation, string token);
        Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID, string token);
        Task<MarketplaceBuyerLocation> Save(string buyerID, string buyerLocationID, MarketplaceBuyerLocation buyerLocation, string token);
        Task Delete(string buyerID, string buyerLocationID, string token);
    }

    public class MarketplaceBuyerLocationCommand : IMarketplaceBuyerLocationCommand
    {
        private IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        public MarketplaceBuyerLocationCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }
        public async Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID, string token)
        {
            var buyerAddress = await _oc.Addresses.GetAsync<MarketplaceAddressBuyer>(buyerID, buyerLocationID, accessToken: token);
            var buyerUserGroup = await _oc.UserGroups.GetAsync<MarketplaceLocationUserGroup>(buyerID, buyerLocationID, accessToken: token);
            return new MarketplaceBuyerLocation
            {
                Address = buyerAddress,
                UserGroup = buyerUserGroup
            };
        }

        public async Task<MarketplaceBuyerLocation> Create(string buyerID, MarketplaceBuyerLocation buyerLocation, string token)
        {
            var buyerLocationID = CreateBuyerLocationID(buyerID, buyerLocation.Address.ID);
            buyerLocation.Address.ID = buyerLocationID;
            var buyerAddress = await _oc.Addresses.CreateAsync<MarketplaceAddressBuyer>(buyerID, buyerLocation.Address, accessToken: token);

            buyerLocation.UserGroup.ID = buyerAddress.ID;
            var buyerUserGroup = await _oc.UserGroups.CreateAsync<MarketplaceLocationUserGroup>(buyerID, buyerLocation.UserGroup, accessToken: token);
            await CreateUserGroupAndAssignments(buyerID, buyerAddress.ID, token);
            await CreateLocationUserGroupsAndApprovalRule(buyerAddress.ID, buyerAddress.AddressName, token);

            return new MarketplaceBuyerLocation
            {
                Address = buyerAddress,
                UserGroup = buyerUserGroup,
            };
        }

        private string CreateBuyerLocationID(string buyerID, string idInRequest)
        {
            if (idInRequest.Contains("LocationIncrementor"))
            {
                // prevents prefix duplication with address validation prewebhooks
                return idInRequest;
            }
            if (idInRequest == null || idInRequest.Length == 0)
            {
                return buyerID + "-{" + buyerID + "-LocationIncrementor}";
            }
            else
            {
                return buyerID + "-" + idInRequest.Replace("-", "_");
            }
        }

        public async Task CreateUserGroupAndAssignments(string buyerID, string buyerLocationID, string token)
        {
            var assignment = new AddressAssignment
            {
                AddressID = buyerLocationID,
                UserGroupID = buyerLocationID,
                IsBilling = true,
                IsShipping = true
            };
            await _oc.Addresses.SaveAssignmentAsync(buyerID, assignment, accessToken: token);
        }

        public async Task<MarketplaceBuyerLocation> Save(string buyerID, string buyerLocationID, MarketplaceBuyerLocation buyerLocation, string token)
        {
            buyerLocation.Address.ID = buyerLocationID;
            buyerLocation.UserGroup.ID = buyerLocationID;
            UserGroup existingLocation = null;
            try
            {
                existingLocation = await _oc.UserGroups.GetAsync(buyerID, buyerLocationID, token);
            } catch (Exception e) { } // Do nothing if not found
            var updatedBuyerAddress = _oc.Addresses.SaveAsync<MarketplaceAddressBuyer>(buyerID, buyerLocationID, buyerLocation.Address, accessToken: token);
            var updatedBuyerUserGroup = _oc.UserGroups.SaveAsync<MarketplaceLocationUserGroup>(buyerID, buyerLocationID, buyerLocation.UserGroup, accessToken: token);
            var location = new MarketplaceBuyerLocation
            {
                Address = await updatedBuyerAddress,
                UserGroup = await updatedBuyerUserGroup,
            };
            if (existingLocation == null)
			{
                var assingments = CreateUserGroupAndAssignments(token, buyerID, buyerLocationID);
                var groups =  CreateLocationUserGroupsAndApprovalRule(token, buyerLocationID, buyerLocation.Address.AddressName);
                await Task.WhenAll(assingments, groups);
            }
            return location;
        }

        public async Task Delete(string buyerID, string buyerLocationID, string token)
        {
            var deleteAddressReq = _oc.Addresses.DeleteAsync(buyerID, buyerLocationID, accessToken: token);
            var deleteUserGroupReq = _oc.UserGroups.DeleteAsync(buyerID, buyerLocationID, accessToken: token);
            await Task.WhenAll(deleteAddressReq, deleteUserGroupReq);
        }

        public async Task CreateLocationUserGroupsAndApprovalRule(string buyerLocationID, string locationName, string token)
        {
            var buyerID = buyerLocationID.Split('-').First();
            var AddUserTypeRequests = SEBUserTypes.BuyerLocation().Select(userType => AddUserTypeToLocation(token, buyerLocationID, userType));
            await Task.WhenAll(AddUserTypeRequests);

            var approvingGroupID = $"{buyerLocationID}-{UserGroupSuffix.OrderApprover.ToString()}";
            await _oc.ApprovalRules.CreateAsync(buyerID, new ApprovalRule()
            {
                ID = buyerLocationID,
                ApprovingGroupID = approvingGroupID,
                Description = "General Approval Rule for Location. Every Order Over a Certain Limit will Require Approval for the designated group of users.",
                Name = $"{locationName} General Location Approval Rule",
                RuleExpression = $"order.xp.ApprovalNeeded = '{buyerLocationID}' & order.Total > 0"
            });
        }

        public async Task AddUserTypeToLocation(string token, string buyerLocationID, MarketplaceUserType marketplaceUserType)
        {
            var buyerID = buyerLocationID.Split('-').First();
            var userGroupID = $"{buyerLocationID}-{marketplaceUserType.UserGroupIDSuffix}";
            await _oc.UserGroups.CreateAsync(buyerID, new PartialUserGroup()
            {
                ID = userGroupID,
                Name = marketplaceUserType.UserGroupName,
                xp = new
                {
                    Role = marketplaceUserType.UserGroupIDSuffix.ToString(),
                    Type = marketplaceUserType.UserGroupType,
                    Location = buyerLocationID
                }
            }, token);
            foreach (var customRole in marketplaceUserType.CustomRoles)
            {
                await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment()
                {
                    BuyerID = buyerID,
                    UserGroupID = userGroupID,
                    SecurityProfileID = customRole.ToString()
                }, token);
            }
        }


    }
}