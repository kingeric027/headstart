using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Marketplace.Common.Services.SmartyStreets.Mappers;
using Marketplace.Models.Misc;
using OrderCloud.SDK;
using SmartyStreets;
using SmartyStreets.USStreetApi;

namespace Marketplace.Common.Services.SmartyStreets
{
	public interface ISmartyStreetsService
	{
		Task<AddressValidation> ValidateAddress(Address address);
		Task<AddressValidation> ValidateAddress(BuyerAddress address);
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

		public async Task<AddressValidation> ValidateAddress(BuyerAddress buyerAddress)
		{
			var address = SmartyStreetMappers.Map(buyerAddress);
			return await ValidateAddress(address);
		}

		public async Task<AddressValidation> ValidateAddress(Address address)
		{
			var response = new AddressValidation()
			{
				RawAddress = address,
				IsRawAddressValid = false,
				AreSuggestionsValid = true
			};
			var candidate = await ValidateSingleUSAddress(address); // Always seems to return 1 or 0 candidates
			if (candidate.Count == 0)
			{
				// Address not valid, no candiates found
				var suggestions = await USAutoCompletePro($"{address.Street1} {address.Street2}");
				response.AreSuggestionsValid = false; // Suggestions from this api do not include zip
				response.SuggestedAddresses = SmartyStreetMappers.Map(suggestions, address);
			}
			// Valid candidate found, but may not match raw exactly. Want to show candidate to user to approve modifications
			else if (CandidateModified(candidate[0])) 
			{
				response.SuggestedAddresses = new List<Address> { SmartyStreetMappers.Map(candidate[0], address) };
			} else
			{
				response.IsRawAddressValid = true;
			}
			return response;
		}

		// returns 1 or 0 very complete addresses
		private async Task<List<Candidate>> ValidateSingleUSAddress(Address address)
		{
			var client = _builder.BuildUsStreetApiClient();
			var lookup = SmartyStreetMappers.MapToUSStreet(address);
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

		private bool CandidateModified(Candidate candidate) 
		{
			// See https://smartystreets.com/docs/cloud/us-street-api#footnotes
			return candidate.Analysis?.Footnotes != null;
		}
	}
}
