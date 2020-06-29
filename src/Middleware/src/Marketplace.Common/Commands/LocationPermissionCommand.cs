using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Models.Misc;
using Marketplace.Common.TemporaryAppConstants;
using ordercloud.integrations.library;

namespace Marketplace.Common.Commands
{
    public interface ILocationPermissionCommand
    {
        Task<List<UserGroupAssignment>> ListLocationPermissionAsssignments(string buyerID, string locationID, VerifiedUserContext verifiedUser);
        Task<List<UserGroupAssignment>> ListLocationApprovalPermissionAsssignments(string buyerID, string locationID, VerifiedUserContext verifiedUser);
        Task<decimal> GetApprovalThreshold(string buyerID, string locationID, VerifiedUserContext verifiedUser);
        Task<decimal> SetLocationApprovalThreshold(string buyerID, string locationID, decimal newApprovalThreshold, VerifiedUserContext verifiedUser);
        Task<ListPage<MarketplaceUser>> ListLocationUsers(string buyerID, string locationID, VerifiedUserContext verifiedUser);
        Task<List<UserGroupAssignment>> UpdateLocationPermissions(string buyerID, string locationID, LocationPermissionUpdate locationPermissionUpdate, VerifiedUserContext verifiedUser);
        Task<bool> IsUserInAccessGroup(string locationID, string groupSuffix, VerifiedUserContext verifiedUser);
    }

    public class LocationPermissionCommand : ILocationPermissionCommand
    {
        private readonly IOrderCloudClient _oc;
        
        public LocationPermissionCommand(IOrderCloudClient oc)
        {
			_oc = oc;
        }

        public async Task<List<UserGroupAssignment>> ListLocationPermissionAsssignments(string buyerID, string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);
            var locationUserTypes = SEBUserTypes.BuyerLocation().Where(s => s.UserGroupIDSuffix != UserGroupSuffix.NeedsApproval.ToString() || s.UserGroupIDSuffix != UserGroupSuffix.OrderApprover.ToString());
            var userGroupAssignmentResponses = await Throttler.RunAsync(locationUserTypes, 100, 5, locationUserType => {
                return _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID: $"{locationID}-{locationUserType.UserGroupIDSuffix}", pageSize: 100);
            });
            return userGroupAssignmentResponses
                .Select(userGroupAssignmentResponse => userGroupAssignmentResponse.Items)
                .SelectMany(l => l)
                .ToList();
        }

        public async Task<decimal> GetApprovalThreshold(string buyerID, string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);
            var approvalRule = await _oc.ApprovalRules.GetAsync(buyerID, locationID);
            var threshold = Convert.ToDecimal(approvalRule.RuleExpression.Split('>')[1]);
            return threshold;
        }

        public async Task<decimal> SetLocationApprovalThreshold(string buyerID, string locationID, decimal newApprovalThreshold, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);
            var approvalRulePatch = new PartialApprovalRule()
            { 
                RuleExpression = $"order.xp.ApprovalNeeded = '{locationID}' & order.Total > {newApprovalThreshold}"
            };
            await _oc.ApprovalRules.PatchAsync(buyerID, locationID, approvalRulePatch);
            return newApprovalThreshold;
        }

        public async Task<List<UserGroupAssignment>> ListLocationApprovalPermissionAsssignments(string buyerID, string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);
            var locationUserTypes = SEBUserTypes.BuyerLocation().Where(s => s.UserGroupIDSuffix == UserGroupSuffix.NeedsApproval.ToString() || s.UserGroupIDSuffix == UserGroupSuffix.OrderApprover.ToString());
            var userGroupAssignmentResponses = await Throttler.RunAsync(locationUserTypes, 100, 5, locationUserType => {
                return _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID: $"{locationID}-{locationUserType.UserGroupIDSuffix}", pageSize: 100);
            });
            return userGroupAssignmentResponses
                .Select(userGroupAssignmentResponse => userGroupAssignmentResponse.Items)
                .SelectMany(l => l)
                .ToList();
        }

        public async Task<List<UserGroupAssignment>> UpdateLocationPermissions(string buyerID, string locationID, LocationPermissionUpdate locationPermissionUpdate, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);

            await Throttler.RunAsync(locationPermissionUpdate.AssignmentsToAdd, 100, 5, assignmentToAdd =>
            {
                return _oc.UserGroups.SaveUserAssignmentAsync(buyerID, assignmentToAdd);
            });
            await Throttler.RunAsync(locationPermissionUpdate.AssignmentsToDelete, 100, 5, assignmentToDelete =>
            {
                return _oc.UserGroups.DeleteUserAssignmentAsync(buyerID, assignmentToDelete.UserGroupID, assignmentToDelete.UserID);
            });

            return await ListLocationPermissionAsssignments(buyerID, locationID, verifiedUser);
        }

        public async Task<ListPage<MarketplaceUser>> ListLocationUsers(string buyerID, string locationID, VerifiedUserContext verifiedUser)
        {
            await EnsureUserIsLocationAdmin(locationID, verifiedUser);
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