﻿using Marketplace.Common.TemporaryAppConstants;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Marketplace.Models.Misc;

namespace Marketplace.Common.Commands
{
    public interface IMarketplaceBuyerCommand
    {
        Task<MarketplaceBuyer> Create(MarketplaceBuyer buyer, VerifiedUserContext user, string token);
    }
    public class MarketplaceBuyerCommand : IMarketplaceBuyerCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;

        public MarketplaceBuyerCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }
        public async Task<MarketplaceBuyer> Create(MarketplaceBuyer buyer, VerifiedUserContext user, string token)
        {
            var ocBuyer = await _oc.Buyers.CreateAsync(buyer, token);
            var ocBuyerID = ocBuyer.ID;

            // create base security profile assignment
            await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment
            {
                BuyerID = ocBuyerID,
                SecurityProfileID = CustomRole.MPBaseBuyer.ToString()
            }, token);

            await CreateUserTypeUserGroupsAndSecurityProfileAssignments(token, ocBuyerID);

            return buyer;

        }

        public async Task CreateUserTypeUserGroupsAndSecurityProfileAssignments(string token, string buyerID)
        {
            foreach (var userType in SEBUserTypes.Buyer())
            {
                var userGroupID = $"{buyerID}{userType.UserGroupIDSuffix}";

                await _oc.UserGroups.CreateAsync(buyerID, new UserGroup()
                {
                    ID = userGroupID,
                    Name = userType.UserGroupName,
                    xp =
                        {
                            Type = "UserPermissions",
                        }
                }, token);

                foreach (var customRole in userType.CustomRoles)
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
}
