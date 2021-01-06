using Marketplace.Common.Models;
using Marketplace.Common.Services.AnytimeDashboard.Models;
using Marketplace.Common.Services.WaxingDashboard.Models;
using Marketplace.Models;

namespace Marketplace.Common.Mappers
{
	public class SyncUser : HSUser
	{
		public bool ShouldSync { get; set; }
		public bool IsAdmin { get; set; }
	}

	public static class UserMapper
	{
		public static SyncUser MapToUser(string buyerID, HSBuyerLocation location, AFStaff staff)
		{
			if (staff == null || location == null) { return null; }
			return new SyncUser()
			{
				ShouldSync = true,
				IsAdmin = staff.type == "Regional Manager" || staff.type == "Owner",
				ID = $"{buyerID}-A{staff.id}",
				FirstName = staff.firstName,
				LastName = staff.lastName,
				Username = $"A-{staff.username}",
				Email = staff.email,
				Active = !staff.isDeleted,
				xp = new UserXp()
				{
					Country = Geography.GetCountry(location.Address.Country)
				}
			};
		}

		public static SyncUser MapToUser(string buyerID, AFGetStaffResponse staff)
		{
			if (staff == null) { return null; }
			return new SyncUser()
			{
				ShouldSync = true,
				IsAdmin = staff.type == "Regional Manager" || staff.type == "Owner",
				ID = $"{buyerID}-A{staff.id}",
				FirstName = staff.firstName,
				LastName = staff.lastName,
				Username = $"A-{staff.username}",
				Email = staff.email,
				Active = !staff.isDeleted,
				xp = new UserXp()
				{
					Country = Geography.GetCountry(staff.homeClub.address.country)
				}
			};
		}

		public static SyncUser MapToUser(string buyerID, WTCStaff staff)
		{
			if (staff == null) { return null; }
			return new SyncUser()
			{
				ShouldSync = staff.userType == "Manager" || staff.userType == "Owner" || staff.userType == "Corporate",
				IsAdmin = true,
				ID = $"{buyerID}-W{staff.id}",
				FirstName = staff.firstName,
				LastName = staff.lastName,
				Username = $"W-{staff.email}",
				Email = staff.email,
				Active = true,
				xp = new UserXp()
				{
					Country = "US"
				}
			};
		}

		public static SSOAuthFields MapToAuthFields(string buyerID, WTCStaff staff)
		{
			var user = MapToUser(buyerID, staff);
			return new SSOAuthFields()
			{
				ID = user.ID,
				Username = user.Username
			};
		}

		public static SSOAuthFields MapToAuthFields(string buyerID, AFCredentials creds)
		{
			return new SSOAuthFields()
			{
				ID = $"{buyerID}-A{creds.authId}", 
				Username = $"A-{creds.username}"
			};
		}
	}
}
