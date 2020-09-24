using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderCloud.SDK;
using Marketplace.Models;
using Marketplace.Models.Misc;
using ordercloud.integrations.library;
using Marketplace.Common.Constants;
using ordercloud.integrations.library.helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        Task<ListPage<MarketplaceLocationUserGroup>> ListUserGroupsByCountry(ListArgs<MarketplaceLocationUserGroup> args, string buyerID, string userID, bool viewAssigned, VerifiedUserContext verifiedUser);
        //Task<ListPage<MarketplaceLocationUserGroup>> ListUserGroupsByCountry(ListArgs<MarketplaceLocationUserGroup> args, string buyerID, string homeCountry, VerifiedUserContext verifiedUser);
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

        public async Task<ListPage<MarketplaceLocationUserGroup>> ListUserGroupsByCountry(ListArgs<MarketplaceLocationUserGroup> args, string buyerID, string userID, bool viewAssigned, VerifiedUserContext verifiedUser)
        {
            var user = await _oc.Users.GetAsync(
                buyerID,
                userID
                );
            var userGroups = new ListPage<MarketplaceLocationUserGroup>();
            if (!viewAssigned)
            {
                userGroups = await _oc.UserGroups.ListAsync<MarketplaceLocationUserGroup>(
                   buyerID,
                   search: args.Search,
                   filters: $"xp.Country={user.xp.Country}&xp.Type=BuyerLocation",
                   page: args.Page,
                   pageSize: 100
                   );
            } else
            {
                var allUserGroups = await ListAllAsync.List((page) => _oc.UserGroups.ListAsync<MarketplaceLocationUserGroup>(
                   buyerID,
                   search: args.Search,
                   filters: $"xp.Country={user.xp.Country}&xp.Type=BuyerLocation",
                   page: page,
                   pageSize: 100
               ));
                var allUserGroupAssignments = await ListAllAsync.List((page) => _oc.UserGroups.ListUserAssignmentsAsync(
                    buyerID,
                    userID: userID
                    ));
                var activeUserLocationAssignments = new List<MarketplaceLocationUserGroup>();
                foreach (var assignment in allUserGroupAssignments)
                {
                    var match = allUserGroups.Find(group => group.ID == assignment.UserGroupID);
                    if (match != null)
                    {
                        activeUserLocationAssignments.Add(match);
                    }
                }
                userGroups.Items = activeUserLocationAssignments;
                userGroups.Meta = new ListPageMeta()
                {
                    Page = 1,
                    PageSize = 100
                };
            }
            //if (viewAssigned)
            return userGroups;
        }

        //public async Task<ListPage<MarketplaceLocationUserGroup>> ListUserGroupsByCountry(ListArgs<MarketplaceLocationUserGroup> args, string buyerID, string homeCountry, VerifiedUserContext user)
        //{
        //    var userGroups = await _oc.UserGroups.ListAsync<MarketplaceLocationUserGroup>(
        //        buyerID,
        //        filters: $"xp.Country={homeCountry}",
        //        search: args.Search,
        //        pageSize: 100,
        //        page: args.Page);

        //    return userGroups;
        //}

    private async Task<bool> IsUserInUserGroup(string buyerID, string userGroupID, VerifiedUserContext verifiedUser)
        {
            var userGroupAssignmentForAccess = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID, verifiedUser.UserID);
            return userGroupAssignmentForAccess.Items.Count > 0;
        }

    };
}