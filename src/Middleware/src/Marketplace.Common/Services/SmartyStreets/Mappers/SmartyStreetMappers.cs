using Marketplace.Common.Extensions;
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
	public static class SmartyStreetMapper
	{
		public static Lookup MapToUSStreetLookup(BridgeAddress address)
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

		public static List<BridgeAddress> Map(AutoCompleteResponse repsonse, BridgeAddress raw)
		{
			var addresses = repsonse.suggestions.Select(suggestion => {
				var rawCopy = JsonConvert.DeserializeObject<BridgeAddress>(JsonConvert.SerializeObject(raw));
				rawCopy.Street1 = suggestion.street_line;
				rawCopy.Street2 = suggestion.secondary;
				rawCopy.City = suggestion.city;
				rawCopy.State = suggestion.state;
				rawCopy.Zip = suggestion.zipcode;
				return rawCopy;
			}).ToList();
			return addresses;
		}

		public static BridgeAddress Map(Candidate candidate, BridgeAddress raw)
		{
			// break reference which was causing pass by reference error
			var rawCopy = JsonConvert.DeserializeObject<BridgeAddress>(JsonConvert.SerializeObject(raw));
			rawCopy.Street1 = candidate.DeliveryLine1;
			rawCopy.Street2 = candidate.DeliveryLine2;
			rawCopy.City = candidate.Components.CityName;
			rawCopy.State = candidate.Components.State;
			rawCopy.Zip = $"{candidate.Components.ZipCode}-{candidate.Components.Plus4Code}";
			return rawCopy;
		}
	}

	public class BridgeAddress
	{
		public string Phone { get; set; }
		public string Country { get; set; }
		public string Zip { get; set; }
		public string State { get; set; }
		public string City { get; set; }
		public string Street2 { get; set; }
		public string Street1 { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string CompanyName { get; set; }
		public string AddressName { get; set; }
		public dynamic xp { get; set; }
	}
}
