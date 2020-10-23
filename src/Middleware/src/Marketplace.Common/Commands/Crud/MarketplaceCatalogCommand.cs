using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Models;
using ordercloud.integrations.library;
using OrderCloud.SDK;

namespace Marketplace.Common.Commands.Crud
{
	public interface IMarketplaceCatalogCommand
	{
		Task<ListPage<MarketplaceCatalog>> List(string buyerID, ListArgs<MarketplaceCatalog> args, VerifiedUserContext user);
		Task<MarketplaceCatalog> Post(string buyerID, MarketplaceCatalog catalog, VerifiedUserContext user);
		Task<ListPage<MarketplaceCatalogAssignment>> GetAssignments(string buyerID, string locationID, VerifiedUserContext user);
		Task SetAssignments(string buyerID, string locationID, List<string> assignments, VerifiedUserContext user);
		Task<MarketplaceCatalog> Get(string buyerID, string catalogID, VerifiedUserContext user);
		Task<MarketplaceCatalog> Put(string buyerID, string catalogID, MarketplaceCatalog catalog, VerifiedUserContext user);
		Task Delete(string buyerID, string catalogID, VerifiedUserContext user);
		Task SyncUserCatalogAssignmentsForUserOnRemoveFrom(string buyerID, string locationID, string userID);
		Task SyncUserCatalogAssignmentsForUserOnAddToLocation(string buyerID, string locationID, string userID);
	}

	public class MarketplaceCatalogCommand : IMarketplaceCatalogCommand
	{
		private readonly IOrderCloudClient _oc;
		public MarketplaceCatalogCommand(AppSettings settings, IOrderCloudClient oc)
		{
			_oc = oc;
		}

		public async Task<MarketplaceCatalog> Get(string buyerID, string catalogID, VerifiedUserContext user)
		{
			return await _oc.UserGroups.GetAsync<MarketplaceCatalog>(buyerID, catalogID, user.AccessToken);
		}

		public async Task<ListPage<MarketplaceCatalog>> List(string buyerID, ListArgs<MarketplaceCatalog> args, VerifiedUserContext user)
		{
			var queryParamsForCatalogUserGroup = new Tuple<string, string>("xp.Type", "Catalog");
			args.Filters.Add(new ListFilter()
			{
				QueryParams = new List<Tuple<string, string>> { queryParamsForCatalogUserGroup }
			});
			return await _oc.UserGroups.ListAsync<MarketplaceCatalog>(buyerID, filters: args.ToFilterString(),
				search: args.Search,
				pageSize: args.PageSize,
				page: args.Page,
				accessToken: user.AccessToken);
		}
		
		public async Task<ListPage<MarketplaceCatalogAssignment>> GetAssignments(string buyerID, string locationID, VerifiedUserContext user)
		{
			// assignments are stored on location usergroup xp in a string array with the ids of the catalogs
			// currently they can only be assessed by location ID
			// limiting to 20 catalog assignments for now

			var location = await _oc.UserGroups.GetAsync<MarketplaceLocationUserGroup>(buyerID, locationID, user.AccessToken);

			var catalogAssignments = new List<MarketplaceCatalogAssignment>{};
			
			if(location.xp.CatalogAssignments != null)
			{
				catalogAssignments = location.xp.CatalogAssignments.Select(catalogIDOnXp => new MarketplaceCatalogAssignment()
					{
						CatalogID = catalogIDOnXp,
						LocationID = locationID
					}).ToList();
			}

			return catalogAssignments.ToListPage(page: 1, pageSize: 100);
		}

		public async Task SetAssignments(string buyerID, string locationID, List<string> newAssignments, VerifiedUserContext user)
		{
			var locationPrePatch = await _oc.UserGroups.GetAsync<MarketplaceLocationUserGroup>(buyerID, locationID, user.AccessToken);
			await _oc.UserGroups.PatchAsync(buyerID, locationID, new PartialUserGroup() { xp = new { CatalogAssignments = newAssignments } }, user.AccessToken);

			// todo consider moving this out of the req res flow with webhooks or queue or something to prevent lengthy call
			await UpdateUserCatalogAssignmentsForLocation(buyerID, locationID, locationPrePatch.xp.CatalogAssignments, newAssignments);
		}

		// logic used when a user is assigned or removed from a location
		// wouldn't be as efficient to use the same logic for this as is used for the catalog location assignment setting
		public async Task SyncUserCatalogAssignmentsForUserOnAddToLocation(string buyerID, string locationID, string userID)
		{
			var location = await _oc.UserGroups.GetAsync<MarketplaceLocationUserGroup>(buyerID, locationID);
			if(location.xp.CatalogAssignments != null && location.xp.CatalogAssignments.Count() > 0)
			{
				var userGroupAssignmentsForUser = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userID: userID, pageSize: 100);
				var catalogAssignmentsToMake = location.xp.CatalogAssignments.Where(catalogID => !userGroupAssignmentsForUser.Items.Any(g => g.UserGroupID == catalogID));
				await Throttler.RunAsync(catalogAssignmentsToMake, 100, 5, catalogAssignmentToMake =>
				{
					return _oc.UserGroups.SaveUserAssignmentAsync(buyerID, new UserGroupAssignment()
					{
						UserGroupID = catalogAssignmentToMake,
						UserID = userID
					});
				});
			}
		}

		public async Task SyncUserCatalogAssignmentsForUserOnRemoveFrom(string buyerID, string locationID, string userID)
		{
			var location = await _oc.UserGroups.GetAsync<MarketplaceLocationUserGroup>(buyerID, locationID);
			if (location.xp.CatalogAssignments != null && location.xp.CatalogAssignments.Count() > 0)
			{
				// todo more than 100 locations
				var locations = await _oc.UserGroups.ListAsync<MarketplaceLocationUserGroup>(buyerID, opts => opts.AddFilter(u => u.xp.Type == "BuyerLocation").PageSize(100));
				var locationUserAssignments = await Throttler.RunAsync(locations.Items, 100, 5, l =>
				{
					return _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID: l.ID, pageSize: 100);

				});
				var locationUserAssignmentsFlat = locationUserAssignments.SelectMany(u => u.Items.ToList());
				var catalogsBuyerShouldSee = locations.Items.Where(l => locationUserAssignmentsFlat.Any(assignment => assignment.UserID == userID && l.ID == assignment.UserGroupID) && l.ID != locationID).SelectMany(l =>
				{
					return l.xp.CatalogAssignments != null && l.xp.CatalogAssignments.Count() > 0 ? l.xp.CatalogAssignments : new List<string>() { };
				});
				var catalogAssignmentsToRemove = location.xp.CatalogAssignments.Where(locationCatalogAssignment => !catalogsBuyerShouldSee.Contains(locationCatalogAssignment));
				await Throttler.RunAsync(catalogAssignmentsToRemove, 100, 5, catalogAssignmentToRemove =>
			   {
				   return _oc.UserGroups.DeleteUserAssignmentAsync(buyerID, catalogAssignmentToRemove, userID);
			   });

			}
		}

		private async Task UpdateUserCatalogAssignmentsForLocation(string buyerID, string locationID, List<string> oldAssignments, List<string> newAssignments)
		{
			try
			{
				var addedAssignments = oldAssignments == null ? newAssignments : newAssignments.Where(newAssignment => !oldAssignments.Contains(newAssignment)).ToList();
				var deletedAssignments = oldAssignments == null ? new List<string>() : oldAssignments.Where(oldAssignment => !newAssignments.Contains(oldAssignment)).ToList();

				var users = await _oc.Users.ListAsync(buyerID, userGroupID: locationID, pageSize: 100);
				if (addedAssignments.Count() > 0)
				{
					var userCatalogAssignments = await Throttler.RunAsync(users.Items, 100, 4, user =>
					{
						// todo > 100
						return GetUserCatalogAssignments(buyerID, user.ID);
					});
					await AssignUsersWhoDontHaveAssignmentForAddedAssignments(buyerID, locationID, addedAssignments, userCatalogAssignments.ToList());
				}

				if(deletedAssignments.Count() > 0)
				{
					// todo more than 100 locations
					var locations = await _oc.UserGroups.ListAsync<MarketplaceLocationUserGroup>(buyerID, opts => opts.AddFilter(u => u.xp.Type == "BuyerLocation").PageSize(100));
					var locationUserAssignments = await Throttler.RunAsync(locations.Items, 100, 5, location =>
					{
						return _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID: location.ID, pageSize: 100);

					});
					await UnassignUsersWhoDontHaveAssignmentsToCatalogsFromOtherLocations(buyerID, locationID, deletedAssignments, users.Items.ToList(), locations.Items.ToList(), locationUserAssignments.SelectMany(u => u.Items.ToList()).ToList());
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

		private async Task UnassignUsersWhoDontHaveAssignmentsToCatalogsFromOtherLocations(string buyerID, string locationID, List<string> deletedAssignments, List<User> users, List<MarketplaceLocationUserGroup> locations, List<UserGroupAssignment> userGroupAssignments)
		{
			await Throttler.RunAsync(deletedAssignments, 100, 5, deletedAssignment =>
			{
				return UnassignUsersWhoDontHaveAssignmentsToCatalogFromOtherLocations(buyerID, locationID, deletedAssignment, users, locations, userGroupAssignments);
			});
		}

		private async Task UnassignUsersWhoDontHaveAssignmentsToCatalogFromOtherLocations(string buyerID, string locationID, string deletedAssignment, List<User> users, List<MarketplaceLocationUserGroup> locations, List<UserGroupAssignment> userGroupAssignments)
		{
			var locationsWithAssignmentToDeletedCatalog = locations.Where(location => location.xp.CatalogAssignments != null && location.xp.CatalogAssignments.Contains(deletedAssignment) && location.ID != locationID).ToList();
			//var usersWhoNeedAssignmentDeleted = users.Where(user => !userGroupAssignments.Any(userGroupAssignment => locationsWithAssignmentToDeletedCatalog.Any(location => userGroupAssignment.UserGroupID))
			var usersWhoNeedAssignmentDeleted = users.Where(user => !IsUserAssignedToLocationWithCatalogAssignment(user.ID, locationsWithAssignmentToDeletedCatalog, userGroupAssignments));

			await Throttler.RunAsync(usersWhoNeedAssignmentDeleted, 100, 5, user =>
			{
				return _oc.UserGroups.DeleteUserAssignmentAsync(buyerID, deletedAssignment, user.ID);
			});
		}

		private bool IsUserAssignedToLocationWithCatalogAssignment(string userID, List<MarketplaceLocationUserGroup> locationsWithAssignmentToDeleteCatalog, List<UserGroupAssignment> userLocationAssignments)
		{
			var locationIDsUserIsAssignedTo = userLocationAssignments.Where(assignment => assignment.UserID == userID).Select(assignment => assignment.UserGroupID).ToList();
			return locationIDsUserIsAssignedTo.Any(locationID => locationsWithAssignmentToDeleteCatalog.Any(location => location.ID == locationID));
		}

		private async Task<Tuple<string, List<UserGroupAssignment>>> GetUserCatalogAssignments(string buyerID, string userID)
		{
			var assignments = await _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userID: userID, pageSize: 100);
			return new Tuple<string, List<UserGroupAssignment>>(userID, assignments.Items.ToList());
		}

		public async Task<MarketplaceCatalog> Post(string buyerID, MarketplaceCatalog catalog, VerifiedUserContext user)
		{
			return await _oc.UserGroups.CreateAsync<MarketplaceCatalog>(buyerID, catalog, user.AccessToken);
		}

		public async Task<MarketplaceCatalog> Put(string buyerID, string catalogID, MarketplaceCatalog catalog, VerifiedUserContext user)
		{
			return await _oc.UserGroups.SaveAsync<MarketplaceCatalog>(buyerID, catalogID, catalog, user.AccessToken);
		}

		public async Task Delete(string buyerID, string catalogID, VerifiedUserContext user)
		{
			await _oc.UserGroups.DeleteAsync(buyerID, catalogID, user.AccessToken);
		}
	}
}
