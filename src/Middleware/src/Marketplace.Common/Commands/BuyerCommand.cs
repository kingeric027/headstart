using Marketplace.Common.TemporaryAppConstants;
using Marketplace.Helpers.Models;
using Marketplace.Models;
using Marketplace.Models.Models.Misc;
using OrderCloud.SDK;
using System.Threading.Tasks;

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
            // Create Buyer with active set to false, checks will need to be performed to ensure that
            // the buyer has everything it needs to be active first
            buyer.Active = false;
            var ocBuyer = await _oc.Buyers.CreateAsync(buyer, token);
            var ocBuyerID = ocBuyer.ID;

            // create base security profile assignment
            await _oc.SecurityProfiles.SaveAssignmentAsync(new SecurityProfileAssignment
            {
                BuyerID = ocBuyerID,
                SecurityProfileID = CustomRole.MPBaseBuyer.ToString()
            });

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
