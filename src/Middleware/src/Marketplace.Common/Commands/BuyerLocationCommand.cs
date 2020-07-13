using Marketplace.Common.TemporaryAppConstants;
using Marketplace.Models;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using System.Linq;
using System.Threading.Tasks;
using ordercloud.integrations.library;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceBuyerLocationCommand
    {
        Task<MarketplaceBuyerLocation> Create(string buyerID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user, string token);
        Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID, VerifiedUserContext user);
        Task<MarketplaceBuyerLocation> Update(string buyerID, string buyerLocationID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user);
        Task Delete(string buyerID, string buyerLocationID, VerifiedUserContext user);
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
        public async Task<MarketplaceBuyerLocation> Get(string buyerID, string buyerLocationID, VerifiedUserContext user)
        {
            var buyerAddress = await _oc.Addresses.GetAsync<MarketplaceAddressBuyer>(buyerID, buyerLocationID, accessToken: user.AccessToken);
            var buyerUserGroup = await _oc.UserGroups.GetAsync<MarketplaceLocationUserGroup>(buyerID, buyerLocationID, accessToken: user.AccessToken);
            return new MarketplaceBuyerLocation
            {
                Address = buyerAddress,
                UserGroup = buyerUserGroup
            };
        }
        public async Task<MarketplaceBuyerLocation> Create(string buyerID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user, string token)
        {
            var buyerLocationID = CreateBuyerLocationID(buyerID, buyerLocation);
            buyerLocation.Address.ID = buyerLocationID;
            var buyerAddress = await _oc.Addresses.CreateAsync<MarketplaceAddressBuyer>(buyerID, buyerLocation.Address, accessToken: user.AccessToken);

            buyerLocation.UserGroup.ID = buyerAddress.ID;
            var buyerUserGroup = await _oc.UserGroups.CreateAsync<MarketplaceLocationUserGroup>(buyerID, buyerLocation.UserGroup, accessToken: user.AccessToken);
            await CreateUserGroupAndAssignments(token, buyerID, buyerUserGroup.ID, buyerAddress.ID);
            await CreateLocationUserGroupsAndApprovalRule(token, buyerAddress.ID, buyerAddress.AddressName);

            return new MarketplaceBuyerLocation
            {
                Address = buyerAddress,
                UserGroup = buyerUserGroup,
            };
        }

        private string CreateBuyerLocationID(string buyerID, MarketplaceBuyerLocation buyerLocation)
        {
            var addressIDInRequest = buyerLocation.Address.ID;
            if (addressIDInRequest.Contains("LocationIncrementor"))
            {
                // prevents prefix duplication with address validation prewebhooks
                return addressIDInRequest;
            }
            if (addressIDInRequest == null || addressIDInRequest.Length == 0)
            {
                return buyerID + "-{" + buyerID + "-LocationIncrementor}";
            }
            else
            {
                return buyerID + "-" + addressIDInRequest;
            }
        }

        public async Task CreateUserGroupAndAssignments(string token, string buyerID, string buyerUserGroupID, string buyerAddressID)
        {
            var assignment = new AddressAssignment
            {
                AddressID = buyerAddressID,
                UserGroupID = buyerUserGroupID,
                IsBilling = true,
                IsShipping = true
            };
            await _oc.Addresses.SaveAssignmentAsync(buyerID, assignment, accessToken: token);
        }

        public async Task<MarketplaceBuyerLocation> Update(string buyerID, string buyerLocationID, MarketplaceBuyerLocation buyerLocation, VerifiedUserContext user)
        {
            var updatedBuyerAddress = await _oc.Addresses.SaveAsync<MarketplaceAddressBuyer>(buyerID, buyerLocation.Address.ID, buyerLocation.Address, accessToken: user.AccessToken);
            var updatedBuyerUserGroup = await _oc.UserGroups.SaveAsync<MarketplaceLocationUserGroup>(buyerID, buyerLocation.UserGroup.ID, buyerLocation.UserGroup, accessToken: user.AccessToken);
            return new MarketplaceBuyerLocation
            {
                Address = updatedBuyerAddress,
                UserGroup = updatedBuyerUserGroup,
            };
        }

        public async Task Delete(string buyerID, string buyerLocationID, VerifiedUserContext user)
        {
            var deleteAddressReq = _oc.Addresses.DeleteAsync(buyerID, buyerLocationID, accessToken: user.AccessToken);
            var deleteUserGroupReq = _oc.UserGroups.DeleteAsync(buyerID, buyerLocationID, accessToken: user.AccessToken);
            await Task.WhenAll(deleteAddressReq, deleteUserGroupReq);
        }

        public async Task CreateLocationUserGroupsAndApprovalRule(string token, string buyerLocationID, string locationName)
        {
            var buyerID = buyerLocationID.Split('-').First();
            foreach (var userType in SEBUserTypes.BuyerLocation())
            {
                await AddUserTypeToLocation(token, buyerLocationID, userType);
            }

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