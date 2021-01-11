using Headstart.Common.Services.AnytimeDashboard.Models;
using Headstart.Common.Services.WaxingDashboard.Models;
using Headstart.Models;
using OrderCloud.SDK;

namespace Headstart.Common.Mappers
{
	public class SyncLocation : HSBuyerLocation
	{
		public string FranchiseeID { get; set; }
		public bool ShouldSync { get; set; }
	}


	public static class LocationMapper
	{
		public static SyncLocation MapToLocation(string buyerID, WTCStudio studio)
		{
			if (studio == null) { return null; }
			var country = Geography.GetCountry(studio.country);
			var currency = Geography.GetCurrency(country);
			var ID = $"{buyerID}-{studio.locationNumber}";
			return new SyncLocation()
			{
				FranchiseeID = studio.locationNumber,
				ShouldSync = studio.status != "Inactive",
				Address = new HSAddressBuyer()
				{
					ID = ID,
					Phone = studio.phoneNumber,
					City = studio.city,
					State = studio.state,
					Zip = studio.postCode,
					Street1 = studio.address1,
					Street2 = studio.address2,
					Country = country,
					AddressName = studio.locationName,
					CompanyName = "Waxing The City",
					xp = new BuyerAddressXP()
					{
						LocationID = $"{buyerID}-{studio.locationNumber}",
						Email = studio.email,
						BillingNumber = null,
						Status = studio.status,
						OpeningDate = studio.openingDate,
						LegalEntity = studio.legalEntity,
						PrimaryContactName = studio.primaryContactName
					}
				},
				UserGroup = new HSLocationUserGroup()
				{
					ID = ID,
					Name = studio.locationName,
					xp = new HSLocationUserGroupXp()
					{
						Country = country,
						Type = "BuyerLocation",
						Currency = currency
					}
				}
			};
		}

		public static SyncLocation MapToLocation(string buyerID, AFClub club)
		{
			if (club == null) { return null; }
			var ID = $"{buyerID}-A{club.id}";
			var country = Geography.GetCountry(club.address.country);
			var currency = Geography.GetCurrency(country);
			return new SyncLocation()
			{
				FranchiseeID = club.id,
				ShouldSync = !club.isDeleted,
				Address = new HSAddressBuyer()
				{
					ID = ID,
					Phone = club.phoneNumber,
					City = club.address.city,
					State = Geography.GetStateAbreviationFromName(club.address.stateProvince),
					Zip = club.address.postCode,
					Street1 = club.address.address,
					Street2 = club.address.address2,
					Country = country,
					AddressName = club.name,
					CompanyName = "Anytime Fitness",
					xp = new BuyerAddressXP()
					{
						LocationID = ID,
						Email = club.email,
						Coordinates = club.coordinates,
						BillingNumber = club.billingNumber,
						Status = club.status.description,
						OpeningDate = club.openingDate,
						LegalEntity = club.legalEntity,
						PrimaryContactName = club.primaryContactName
					}
				},
				UserGroup = new HSLocationUserGroup()
				{
					ID = ID,
					Name = club.name,
					xp = new HSLocationUserGroupXp()
					{
						Country = country,
						Type = "BuyerLocation",
						Currency = currency
					}
				}
			};
		}

		// Set only the fields that the sync should overwrite in a patch
		public static PartialAddress ToPartial(this HSAddressBuyer address)
		{
			if (address == null) { return null; }
			return new PartialAddress()
			{
				ID = address.ID,
				Phone = address.Phone,
				City = address.City,
				State = address.State,
				Zip = address.Zip,
				Street1 = address.Street1,
				Street2 = address.Street2,
				Country = address.Country,
				AddressName = address.AddressName,
				CompanyName = address.CompanyName,
				xp = new 
				{
					address.xp.LocationID,
					address.xp.Email,
					address.xp.Coordinates,
					address.xp.BillingNumber,
					address.xp.Status,
					address.xp.OpeningDate,
					address.xp.LegalEntity,
					address.xp.PrimaryContactName
				}
			};	
		}
	}
}
