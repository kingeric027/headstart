using Marketplace.Models;
using Newtonsoft.Json;
using OrderCloud.SDK;
using SmartyStreets.USStreetApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lookup = SmartyStreets.USStreetApi.Lookup;

namespace Marketplace.Common.Services.SmartyStreets.Mappers
{
	// TODO - exact same logic as BuyerAddressMapper - any way to merge?
	public static class AddressMapper
	{
		public static Lookup MapToUSStreetLookup(Address address)
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
			var addresses = repsonse.suggestions.Select(suggestion => {
				var rawCopy = JsonConvert.DeserializeObject<Address>(JsonConvert.SerializeObject(raw));
				rawCopy.Street1 = suggestion.street_line;
				rawCopy.Street2 = suggestion.secondary;
				rawCopy.City = suggestion.city;
				rawCopy.State = suggestion.state;
				rawCopy.Zip = suggestion.zipcode;
				return rawCopy;
			}).ToList();
			return addresses;
		}

		public static Address Map(Candidate candidate, Address raw)
		{
			// break reference which was causing pass by reference error
			var rawCopy = JsonConvert.DeserializeObject<Address>(JsonConvert.SerializeObject(raw));
			rawCopy.Street1 = candidate.DeliveryLine1;
			rawCopy.Street2 = candidate.DeliveryLine2;
			rawCopy.City = candidate.Components.CityName;
			rawCopy.State = candidate.Components.State;
			rawCopy.Zip = $"{candidate.Components.ZipCode}-{candidate.Components.Plus4Code}";
			return rawCopy;
		}
	}
}
