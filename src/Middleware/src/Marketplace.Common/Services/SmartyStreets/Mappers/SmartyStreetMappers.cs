using Marketplace.Common.Extensions;
using Marketplace.Models;
using OrderCloud.SDK;
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

		public static List<Address> Map(AutoCompleteResponse repsonse, Address raw)
		{
			var addresses = repsonse.suggestions.Select(suggestion => Map(suggestion, raw)).ToList();
			return addresses;
		}

		public static Address Map(AutoCompleteSuggestion suggestion, Address raw)
		{
			raw.Street1 = suggestion.street_line;
			raw.Street2 = suggestion.secondary;
			raw.City = suggestion.city;
			raw.State = suggestion.state;
			raw.Zip = suggestion.zipcode;
			return raw;
		}

		public static Address Map(Candidate candidate, Address raw)
		{
			raw.Street1 = candidate.DeliveryLine1;
			raw.Street2 = candidate.DeliveryLine2;
			raw.City = candidate.Components.CityName;
			raw.State = candidate.Components.State;
			raw.Zip = $"{candidate.Components.ZipCode}-{candidate.Components.Plus4Code}";
			return raw;
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
