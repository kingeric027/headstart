using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderCloud.SDK;
using Marketplace.Common.Commands.Zoho;
using Marketplace.Common.Services.FreightPop;
using Marketplace.Models;
using Marketplace.Models.Extended;
using Marketplace.Common.Services;
using Marketplace.Common.Services.Avalara;
using Marketplace.Common.Services.ShippingIntegration.Models;
using Marketplace.Models.Models.Marketplace;
using ordercloud.integrations.extensions;
using Marketplace.Common.Services.ShippingIntegration;
using Marketplace.Models.Misc;
using Marketplace.Common.TemporaryAppConstants;

namespace Marketplace.Common.Commands
{
    public interface ILocationPermissionCommand
    {
        Task<List<UserGroupAssignment>> ListLocationPermissionAsssignments(string locationID, VerifiedUserContext verifiedUser);
        Task<ListPage<MarketplaceUser>> ListLocationUsers(string locationID, VerifiedUserContext verifiedUser);

        Task<bool> IsUserInAccessGroup(string locationID, string groupSuffix, VerifiedUserContext verifiedUser);
    }

    public class LocationPermissionCommand : ILocationPermissionCommand
    {
        private readonly IOrderCloudClient _oc;
        
        public LocationPermissionCommand(IOrderCloudClient oc)
        {
			_oc = oc;
        }

        public async Task<List<UserGroupAssignment>> ListLocationPermissionAsssignments(string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);
            var buyerID = locationID.Split('-')[0];
            var locationUserTypes = SEBUserTypes.BuyerLocation();
            var userGroupAssignmentResponses = await Throttler.RunAsync(locationUserTypes, 100, 5, locationUserType => {
                return _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID: $"{locationID}-{locationUserType.UserGroupIDSuffix}", pageSize: 100);
            });
            return userGroupAssignmentResponses
                .Select(userGroupAssignmentResponse => userGroupAssignmentResponse.Items)
                .SelectMany(l => l)
                .ToList();
        }

        public async Task<ListPage<MarketplaceUser>> ListLocationUsers(string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);
            var buyerID = locationID.Split('-')[0];
            return await _oc.Users.ListAsync<MarketplaceUser>(buyerID, userGroupID: locationID);

        }

        public async Task EnsureUserIsLocationAdmin(string locationID, VerifiedUserContext verifiedUser)
        {
            var hasAccess = await IsUserInAccessGroup(locationID, UserGroupSuffix.PermissionAdmin.ToString(), verifiedUser);
            Require.That(hasAccess, new ErrorCode("Insufficient Access", 403, $"User cannot manage permissions for: {locationID}"));
        }

        public async Task<bool> IsUserInAccessGroup(string locationID, string groupSuffix, VerifiedUserContext verifiedUser)
        {
            var buyerID = verifiedUser.BuyerID;
            var userGroupID = $"{locationID}-{groupSuffix}";
            return await IsUserInUserGroup(buyerID, userGroupID, verifiedUser);
        }

        private async Task<bool> IsUserInUserGroup(string buyerID, string userGroupID, VerifiedUserContext verifiedUser)
        {
            var userGroupAssignmentForAccess = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID, verifiedUser.UserID);
            return userGroupAssignmentForAccess.Items.Count > 0;
        }

    };
}