using OrderCloud.SDK;
using SmartyStreets.USAutocompleteApi;
using SmartyStreets.USStreetApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lookup = SmartyStreets.USStreetApi.Lookup;

namespace Marketplace.Common.Services.SmartyStreets.Mappers
{
	public static class SmartyStreetMappers
	{
		public static Lookup MapToUSStreet(Address address)
		{
			var lookup = new Lookup()
			{
				Street = address.Street1,
				Street2 = address.Street2,
				City = address.City,
				State = address.State,
				ZipCode = address.Zip,
				MatchStrategy = Lookup.STRICT,
				MaxCandidates = 5
			};
			return lookup;
		}

		public static List<Address> Map(AutoCompleteResponse repsonse)
		{
			var addresses = repsonse.suggestions.Select(suggestion => Map(suggestion)).ToList();
			return addresses;
		}

		public static Address Map(Suggestion suggestion)
		{
			var address = new Address()
			{
				Street1 = suggestion.StreetLine,
				City = suggestion.City,
				State = suggestion.State,
				Country = "US"
			};
			return address;
		}

		public static Address Map(Candidate candidate)
		{
			var address = new Address()
			{
				Street1 = candidate.DeliveryLine1,
				Street2 = candidate.DeliveryLine2,
				City = candidate.Components.CityName,
				State = candidate.Components.State,
				Zip = $"{candidate.Components.ZipCode}-{candidate.Components.Plus4Code}",
				Country = "US"
			};
			return address;
		}

		public static Address Map(BuyerAddress buyerAddress)
		{
			var address = new Address()
			{
				ID = buyerAddress.ID,
				DateCreated = buyerAddress.DateCreated,
				CompanyName = buyerAddress.CompanyName,
				FirstName = buyerAddress.FirstName,
				LastName = buyerAddress.LastName,
				Street1 = buyerAddress.Street1,
				Street2 = buyerAddress.Street2,
				City = buyerAddress.City,
				State = buyerAddress.State,
				Zip = buyerAddress.Zip,
				Country = buyerAddress.Country,
				Phone = buyerAddress.Phone,
				AddressName = buyerAddress.AddressName,
				xp = buyerAddress.xp
			};
			return address;
		}
	}
}
