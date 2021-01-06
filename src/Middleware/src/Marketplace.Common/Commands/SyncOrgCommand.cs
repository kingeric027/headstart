using Common.Services.AnytimeDashboard;
using Common.Services.NProg;
using Flurl.Http;
using Marketplace.Common;
using Marketplace.Common.Commands;
using Marketplace.Common.Commands.Crud;
using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using Marketplace.Common.Services.WazingDashboard;
using Microsoft.Extensions.Logging;
using ordercloud.integrations.library;
using ordercloud.integrations.library.helpers;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Common.Commands
{
	public interface ISyncOrgCommand
	{
		Task Run(ILogger log, FranchiseEnum franchise);
		Task SyncOneStudio(ILogger log, string studioID);
		Task SyncOneClub(ILogger log, string clubID);
	}

	public class SyncOrgCommand: ISyncOrgCommand
	{
		private readonly IOrderCloudClient _oc;
		private readonly IMarketplaceBuyerLocationCommand _locationCommand;
		private readonly IMarketplaceCatalogCommand _catalogCommand;
		private readonly IWaxDashboardClient _waxDashboardClient;
		private readonly IAnytimeDashboardClient _anytimeClient;
		private readonly AppSettings _settings;
		private ILogger _log;
		private IFranchiseAPI _franchise;

		private int Skipped = 0;
		private int Succeeded = 0;
		private int Failed = 0;
		private int Total => Skipped + Succeeded + Failed;
		private readonly HashSet<string> AlreadySyncedUserIDs = new HashSet<string>();
		private readonly Tracker _tracker = new Tracker();
		private readonly string[] AdminPermissionUserGroupSuffixs = { "PermissionAdmin", "ResaleCertAdmin", "OrderApprover", "ViewAllOrders", "CreditCardAdmin", "AddressAdmin" };

		public SyncOrgCommand(IOrderCloudClient oc, IMarketplaceBuyerLocationCommand locationCommand, IMarketplaceCatalogCommand catalogCommand, AppSettings settings, IWaxDashboardClient waxDashboardClient, IAnytimeDashboardClient anytimeClient)
		{
			_oc = oc;
			_locationCommand = locationCommand;
			_catalogCommand = catalogCommand;
			_anytimeClient = anytimeClient;
			_settings = settings;
			_waxDashboardClient = waxDashboardClient;
		}

		public async Task Run(ILogger log, FranchiseEnum franchise)
		{
			_log = log;
			_franchise = franchise.GetIFranchise(_settings);
			_tracker.Every(30.Seconds(), p => LogProgress());
			_tracker.Start();
			_log.LogInformation($"Starting -- {_franchise.BrandName} Sync");
			await _franchise.ProcessAllLocations(ProcessLocation);
			LogProgress();
			_log.LogInformation($"Finished -- {_franchise.BrandName} Sync");
		}

		public async Task SyncOneStudio(ILogger log, string studioID)
		{
			_log = log;
			_franchise = FranchiseEnum.WaxingTheCity.GetIFranchise(_settings);
			var studio = await _waxDashboardClient.GetStudioAsync(studioID);
			var location = LocationMapper.MapToLocation("0005", studio.items[0]);
			await ProcessLocation(location);
		}

		public async Task SyncOneClub(ILogger log, string clubID)
		{
			_log = log;
			_franchise = FranchiseEnum.AnytimeFitness.GetIFranchise(_settings);
			var club = await _anytimeClient.GetClub(clubID);
			var location = LocationMapper.MapToLocation("0006", club);
			await ProcessLocation(location);
		}

		private async Task ProcessLocation(SyncLocation location)
		{	
			try 
			{
				if (location == null) 
				{ 
					LogError("", "Location is null");
					return;
				}
				if (!location.ShouldSync)
				{
					LogSkip(location.Address.ID, "Location is inactivated in source data.");
					return;
				}
				if (location.Address.Street1 == null)
				{
					LogSkip(location.Address.ID, "Street1 is null");
					return;
				}
				if (location.Address.Zip == null)
				{
					LogSkip(location.Address.ID, "Zip is null");
					return;
				}

				var users = (await _franchise.ListAllUsersOnLocation(location))
								.Where(user => user.ShouldSync);
				var newUsers = users.Where(u => !AlreadySyncedUserIDs.Contains(u.ID));
				var catalogIDs = _franchise.GetOrderCloudCatalogsIDs(location);
				await SaveLocation(_franchise.BuyerID, location, users, newUsers, catalogIDs);
				foreach (var user in newUsers)
				{
					AlreadySyncedUserIDs.Add(user.ID);
				}
				LogSuccess(location.Address.ID);
			}
			catch (FranchiseAPIException ex)
			{
				LogError(location.Address.ID, ex.Message);
			}
			catch (OrderCloudException ex)
			{
				var response = await ((FlurlHttpException)ex.InnerException).Call.Response.Content.ReadAsStringAsync();
				LogError(location.Address.ID, $"{ ex.InnerException.Message}. Response: { response}");
			}
			catch (Exception ex)
			{
				_log.LogError(location.Address.ID, ex.Message);
			}
		}

		public async Task SaveLocation(string buyerID, SyncLocation location, IEnumerable<SyncUser> users, IEnumerable<SyncUser> newUsers, IEnumerable<string> catalogIDs)
		{
			var locationExists = await LocationExists(buyerID, location);
			if (locationExists)
			{
				// patch it to avoid overwriting non-sync fields
				await _oc.Addresses.PatchAsync(buyerID, location.Address.ID, location.Address.ToPartial());
			}
			else
			{
				 await _locationCommand.Create(buyerID, location, _oc.TokenResponse.AccessToken);
			}
			// TODO - don't just PUT all users.
			await Throttler.RunAsync(newUsers, 100, 8, user => _oc.Users.SaveAsync(buyerID, user.ID, user));
			// Update User-Location Assignments
			var allUserGroups = await ListAllAsync.List(page => _oc.UserGroups.ListUserAssignmentsAsync(buyerID, userGroupID: location.Address.ID, page: page, pageSize: 100));
			var userIDsCurrentlyAssigned = allUserGroups.Select(a => a.UserID);
			var usersToAssign = users.Where(u => !userIDsCurrentlyAssigned.Contains(u.ID));
			var userIDsToUnAssign = userIDsCurrentlyAssigned.Except(users.Select(u => u.ID));
			await Throttler.RunAsync(usersToAssign, 100, 8,
				user => _oc.UserGroups.SaveUserAssignmentAsync(buyerID, new UserGroupAssignment { UserID = user.ID, UserGroupID = location.Address.ID })
			);
			await Throttler.RunAsync(userIDsToUnAssign, 100, 8,
				userID => _oc.UserGroups.DeleteUserAssignmentAsync(buyerID, location.Address.ID, userID)
			);
			// Assign users to Permision UserGroups 
			var permissionAssignments = usersToAssign.Where(s => s.IsAdmin).SelectMany(admin =>
			{
				return AdminPermissionUserGroupSuffixs.Select(suffix =>
				{
					var groupID = $"{location.Address.ID}-{suffix}";
					var assignment = new UserGroupAssignment() { UserID = admin.ID, UserGroupID = groupID };
					return _oc.UserGroups.SaveUserAssignmentAsync(buyerID, assignment);
				});
			});
			await Throttler.RunAsync(permissionAssignments, 100, 8, x => x);

			// Create / update Location-Catalog Assignments
			await _catalogCommand.SetAssignments(buyerID, location.Address.ID, catalogIDs.ToList(), _oc.TokenResponse.AccessToken);
		}

		private async Task<bool> LocationExists(string buyerID, SyncLocation location)
		{
			try
			{
				await _oc.Addresses.GetAsync(buyerID, location.Address.ID);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void LogError(string locationID, string message = "")
		{
			Failed++;
			_log.LogError(FormatMessage("Fail", locationID, message));
		}

		private void LogSkip(string locationID, string message = "")
		{
			Skipped++;
			_log.LogWarning(FormatMessage("Skip", locationID, message));
		}

		private void LogSuccess(string locationID, string message = "")
		{
			Succeeded++;
			_log.LogInformation(FormatMessage("Success", locationID, message));
		}

		private void LogProgress()
		{
			var p = _tracker.GetProgress();
			_log.LogInformation($"Found : {Total}. Failed: {Failed}. Skipped: {Skipped}. Succeeded: {Succeeded}. Time Elapsed: {p.ElapsedTime:hh\\:mm\\:ss}");
		}

		private string FormatMessage(string status, string locationID, string message)
		{
			return locationID.PadRight(14, ' ') + status.ToString().PadRight(10, ' ') + message;
		}
	}
}
