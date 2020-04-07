using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Marketplace.Common.Services.SmartyStreets.Mappers;
using Marketplace.Models.Misc;
using Newtonsoft.Json;
using OrderCloud.SDK;
using SmartyStreets;
using SmartyStreets.USStreetApi;

namespace Marketplace.Common.Services.SmartyStreets
{
	public interface ISmartyStreetsService
	{
		Task<AddressValidation<Address>> ValidateAddress(Address address);
		Task<AddressValidation<BuyerAddress>> ValidateAddress(BuyerAddress address);
	}

	public class SmartyStreetsService : ISmartyStreetsService
	{
		private readonly SmartyStreetSettings _smartySettings;
		private readonly ClientBuilder _builder;
		private readonly string AutoCompleteBaseUrl = "https://us-autocomplete-pro.api.smartystreets.com";

		public SmartyStreetsService(AppSettings settings)
		{
			_smartySettings = settings.SmartyStreetSettings;
			_builder = new ClientBuilder(_smartySettings.AuthID, _smartySettings.AuthToken);
		}

		public async Task<AddressValidation<TAddress>> ValidateAddress<TAddress>(TAddress address)
		{
			var response = new AddressValidation<TAddress>(address);
			var lookup = SmartyStreetBuyerAddressMapper.MapToUSStreetLookup(address);
			var candidate = await ValidateSingleUSAddress(lookup); // Always seems to return 1 or 0 candidates
			if (candidate.Count > 0)
			{
				response.ValidAddress = SmartyStreetBuyerAddressMapper.Map(candidate[0], address);
				response.GapBeteenRawAndValid = candidate[0].Analysis.DpvFootnotes;
			}
			else
			{
				// no valid address found
				var suggestions = await USAutoCompletePro($"{address.Street1} {address.Street2}");
				response.SuggestedAddresses = SmartyStreetBuyerAddressMapper.Map(suggestions, address);
			}
			return response;
		}

		public async Task<AddressValidation> ValidateAddress(Address address)
		{
			var response = new AddressValidation(address);
			var lookup = SmartyStreetAddressMapper.MapToUSStreetLookup(address);
			var candidate = await ValidateSingleUSAddress(lookup); // Always seems to return 1 or 0 candidates
			if (candidate.Count > 0)
			{
				response.ValidAddress = SmartyStreetAddressMapper.Map(candidate[0], address);
				response.GapBeteenRawAndValid = candidate[0].Analysis.DpvFootnotes;
			}
			else 
			{
				// no valid address found
				var suggestions = await USAutoCompletePro($"{address.Street1} {address.Street2}");
				response.SuggestedAddresses = SmartyStreetAddressMapper.Map(suggestions, address);
			}
			return response;
		}

		// returns 1 or 0 very complete addresses
		private async Task<List<Candidate>> ValidateSingleUSAddress(Lookup lookup)
		{
			var client = _builder.BuildUsStreetApiClient();
			client.Send(lookup);
			return await Task.FromResult(lookup.Result);
		}

		// returns many incomplete address suggestions
		private async Task<AutoCompleteResponse> USAutoCompletePro(string search)
		{
			var suggestions = await AutoCompleteBaseUrl
				.AppendPathSegment("lookup")
				.SetQueryParam("key", _smartySettings.WebsiteKey)
				.SetQueryParam("search", search)
				.WithHeader("Referer", _smartySettings.RefererHost)
				.GetJsonAsync<AutoCompleteResponse>();

			return suggestions;
		}
	}
}
