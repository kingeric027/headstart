using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Headstart.Models;
using ordercloud.integrations.library;
using ordercloud.integrations.library.helpers;
using OrderCloud.SDK;

namespace Headstart.Common.Commands.Crud
{
	public interface IHSCatalogCommand
	{
		Task<ListPage<HSCatalog>> List(string buyerID, ListArgs<HSCatalog> args, VerifiedUserContext user);
		Task<HSCatalog> Post(string buyerID, HSCatalog catalog, VerifiedUserContext user);
		Task<ListPage<HSCatalogAssignment>> GetAssignments(string buyerID, string locationID, VerifiedUserContext user);
		Task SetAssignments(string buyerID, string locationID, List<string> assignments, string token);
		Task<HSCatalog> Get(string buyerID, string catalogID, VerifiedUserContext user);
		Task<HSCatalog> Put(string buyerID, string catalogID, HSCatalog catalog, VerifiedUserContext user);
		Task Delete(string buyerID, string catalogID, VerifiedUserContext user);
		Task SyncUserCatalogAssignments(string buyerID, string userID);
	}

	public class HSCatalogCommand : IHSCatalogCommand
	{
		private readonly IOrderCloudClient _oc;
		public HSCatalogCommand(AppSettings settings, IOrderCloudClient oc)
		{
			_oc = oc;
		}

		public async Task<HSCatalog> Get(string buyerID, string catalogID, VerifiedUserContext user)
		{
			return await _oc.UserGroups.GetAsync<HSCatalog>(buyerID, catalogID, user.AccessToken);
		}

		public async Task<ListPage<HSCatalog>> List(string buyerID, ListArgs<HSCatalog> args, VerifiedUserContext user)
		{
			var queryParamsForCatalogUserGroup = new Tuple<string, string>("xp.Type", "Catalog");
			args.Filters.Add(new ListFilter()
			{
				QueryParams = new List<Tuple<string, string>> { queryParamsForCatalogUserGroup }
			});
			return await _oc.UserGroups.ListAsync<HSCatalog>(buyerID, filters: args.ToFilterString(),
				search: args.Search,
				pageSize: args.PageSize,
				page: args.Page,
				accessToken: user.AccessToken);
		}
		
		public async Task<ListPage<HSCatalogAssignment>> GetAssignments(string buyerID, string locationID, VerifiedUserContext user)
		{
			// assignments are stored on location usergroup xp in a string array with the ids of the catalogs
			// currently they can only be assessed by location ID
			// limiting to 20 catalog assignments for now

			var location = await _oc.UserGroups.GetAsync<HSLocationUserGroup>(buyerID, locationID, user.AccessToken);

			var catalogAssignments = new List<HSCatalogAssignment>{};
			
			if(location.xp.CatalogAssignments != null)
			{
				catalogAssignments = location.xp.CatalogAssignments.Select(catalogIDOnXp => new HSCatalogAssignment()
					{
						CatalogID = catalogIDOnXp,
						LocationID = locationID
					}).ToList();
			}

			return catalogAssignments.ToListPage(page: 1, pageSize: 100);
		}

		public async Task SetAssignments(string buyerID, string locationID, List<string> newAssignments, string token)
		{
			var locationPrePatch = await _oc.UserGroups.GetAsync<HSLocationUserGroup>(buyerID, locationID, token);
			await _oc.UserGroups.PatchAsync(buyerID, locationID, new PartialUserGroup() { xp = new { CatalogAssignments = newAssignments } }, token);

			// todo consider moving this out of the req res flow with webhooks or queue or something to prevent lengthy call
			await UpdateUserCatalogAssignmentsForLocation(buyerID, locationID, locationPrePatch.xp.CatalogAssignments, newAssignments);
		}

		//	This function looks at all catalog-user-group ids on the xp.CatalogAssignments array of all assigned BuyerLocation usergroups
		//	Then we add or remove usergroup assignments so the actual assignments allign with what is in the BuyerLocation usergroups
		public async Task SyncUserCatalogAssignments(string buyerID, string userID)
        {
			var currentAssignments = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID: buyerID, userID: userID);
			var currentAssignedCatalogIDs = currentAssignments?.Items?.Select(assignment => assignment?.UserGroupID)?.ToList();
			var currentUserGroups = await _oc.UserGroups.ListAsync<HSLocationUserGroup>(buyerID: buyerID, filters: $"ID={string.Join("|", currentAssignedCatalogIDs)}");
			var catalogsUserShouldSee = currentUserGroups?.Items?.Where(item => (item?.xp?.Type == "BuyerLocation"))?.SelectMany(c => c?.xp?.CatalogAssignments);

			var actualCatalogAssignments = currentUserGroups?.Items?.Where(item => item?.xp?.Type == "Catalog")?.Select(c => c.ID)?.ToList();
			//now remove all actualCatalogAssignments that are not included in catalogsUserShouldSee
			var assignmentsToRemove = actualCatalogAssignments?.Where(id => !catalogsUserShouldSee.Contains(id));
			var assignmentsToAdd = catalogsUserShouldSee?.Where(id => !actualCatalogAssignments.Contains(id));
			await Throttler.RunAsync(assignmentsToRemove, 100, 5, catalogAssignmentToRemove =>
			{
				return _oc.UserGroups.DeleteUserAssignmentAsync(buyerID, catalogAssignmentToRemove, userID);
			});
			await Throttler.RunAsync(assignmentsToAdd, 100, 5, catalogAssignmentToAdd =>
			{
				return _oc.UserGroups.SaveUserAssignmentAsync(buyerID, new UserGroupAssignment()
                {
					UserGroupID = catalogAssignmentToAdd,
					UserID = userID
                });
			});
		}

		private async Task UpdateUserCatalogAssignmentsForLocation(string buyerID, string locationID, List<string> oldAssignments, List<string> newAssignments)
		{
			try
			{
				var addedAssignments = oldAssignments == null ? newAssignments : newAssignments.Where(newAssignment => !oldAssignments.Contains(newAssignment)).ToList();
				var deletedAssignments = oldAssignments == null ? new List<string>() : oldAssignments.Where(oldAssignment => !newAssignments.Contains(oldAssignment)).ToList();

				var users = await ListAllAsync.List((page) => _oc.Users.ListAsync<HSUser>(buyerID, userGroupID: locationID, page: page, pageSize: 100));
				if (addedAssignments.Count() > 0)
				{
					var userCatalogAssignments = await Throttler.RunAsync(users, 100, 4, user =>
					{
						return GetUserCatalogAssignments(buyerID, user.ID);
					});
					await AssignUsersWhoDontHaveAssignmentForAddedAssignments(buyerID, locationID, addedAssignments, userCatalogAssignments.ToList());
				}

				if(deletedAssignments.Count() > 0)
				{
					var locations = await ListAllAsync.List((page) => _oc.UserGroups.ListAsync<HSLocationUserGroup>(buyerID, opts => {
							opts.Page(page);
							opts.AddFilter(u => u.xp.Type == "BuyerLocation").PageSize(100);
						}
					));
					var locationUserAssignments = await Throttler.RunAsync(locations, 100, 5, location =>
					{
						return _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID: location.ID, pageSize: 100);

					});
					await UnassignUsersWhoDontHaveAssignmentsToCatalogsFromOtherLocations(buyerID, locationID, deletedAssignments, users.ToList(), locations.ToList(), locationUserAssignments.SelectMany(u => u.Items.ToList()).ToList());
				}
			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);	
			}

		}

		private async Task AssignUsersWhoDontHaveAssignmentForAddedAssignments(string buyerID, string locationID, List<string> addedAssignments, List<Tuple<string, List<UserGroupAssignment>>> userUserGroupAssignments)
		{
			await Throttler.RunAsync(addedAssignments, 100, 5, addedAssignment =>
			{
				return Throttler.RunAsync(userUserGroupAssignments, 100, 5, userTuple =>
				{
					(string userID, List<UserGroupAssignment> existingAssignments) = userTuple;
					if (!existingAssignments.Any(existingAssignment => existingAssignment.UserGroupID == addedAssignment)) {
						return _oc.UserGroups.SaveUserAssignmentAsync(buyerID, new UserGroupAssignment()
						{
							UserGroupID = addedAssignment,
							UserID = userID
						});
					} else
					{
						// return empty task no need to assign user
						return Task.FromResult<object>(null);
					}
				});
			});
		}

		private async Task UnassignUsersWhoDontHaveAssignmentsToCatalogsFromOtherLocations(string buyerID, string locationID, List<string> deletedAssignments, List<HSUser> users, List<HSLocationUserGroup> locations, List<UserGroupAssignment> userGroupAssignments)
		{
			await Throttler.RunAsync(deletedAssignments, 100, 5, deletedAssignment =>
			{
				return UnassignUsersWhoDontHaveAssignmentsToCatalogFromOtherLocations(buyerID, locationID, deletedAssignment, users, locations, userGroupAssignments);
			});
		}

		private async Task UnassignUsersWhoDontHaveAssignmentsToCatalogFromOtherLocations(string buyerID, string locationID, string deletedAssignment, List<HSUser> users, List<HSLocationUserGroup> locations, List<UserGroupAssignment> userGroupAssignments)
		{
			var locationsWithAssignmentToDeletedCatalog = locations.Where(location => location.xp.CatalogAssignments != null && location.xp.CatalogAssignments.Contains(deletedAssignment) && location.ID != locationID).ToList();
			//var usersWhoNeedAssignmentDeleted = users.Where(user => !userGroupAssignments.Any(userGroupAssignment => locationsWithAssignmentToDeletedCatalog.Any(location => userGroupAssignment.UserGroupID))
			var usersWhoNeedAssignmentDeleted = users.Where(user => !IsUserAssignedToLocationWithCatalogAssignment(user.ID, locationsWithAssignmentToDeletedCatalog, userGroupAssignments));

			await Throttler.RunAsync(usersWhoNeedAssignmentDeleted, 100, 5, user =>
			{
				return _oc.UserGroups.DeleteUserAssignmentAsync(buyerID, deletedAssignment, user.ID);
			});
		}

		private bool IsUserAssignedToLocationWithCatalogAssignment(string userID, List<HSLocationUserGroup> locationsWithAssignmentToDeleteCatalog, List<UserGroupAssignment> userLocationAssignments)
		{
			var locationIDsUserIsAssignedTo = userLocationAssignments.Where(assignment => assignment.UserID == userID).Select(assignment => assignment.UserGroupID).ToList();
			return locationIDsUserIsAssignedTo.Any(locationID => locationsWithAssignmentToDeleteCatalog.Any(location => location.ID == locationID));
		}

		private async Task<Tuple<string, List<UserGroupAssignment>>> GetUserCatalogAssignments(string buyerID, string userID)
		{
			var assignments = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userID: userID, pageSize: 100);
			return new Tuple<string, List<UserGroupAssignment>>(userID, assignments.Items.ToList());
		}

		public async Task<HSCatalog> Post(string buyerID, HSCatalog catalog, VerifiedUserContext user)
		{
			return await _oc.UserGroups.CreateAsync<HSCatalog>(buyerID, catalog, user.AccessToken);
		}

		public async Task<HSCatalog> Put(string buyerID, string catalogID, HSCatalog catalog, VerifiedUserContext user)
		{
			return await _oc.UserGroups.SaveAsync<HSCatalog>(buyerID, catalogID, catalog, user.AccessToken);
		}

		public async Task Delete(string buyerID, string catalogID, VerifiedUserContext user)
		{
			await _oc.UserGroups.DeleteAsync(buyerID, catalogID, user.AccessToken);
		}
	}
}
