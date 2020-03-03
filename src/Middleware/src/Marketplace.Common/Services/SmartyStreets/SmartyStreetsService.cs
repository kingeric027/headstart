using Marketplace.Common.Services.SmartyStreets.models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartyStreets;
using SmartyStreets.USStreetApi;
using SmartyStreets.USAutocompleteApi;
using AutoCompleteLookup = SmartyStreets.USAutocompleteApi.Lookup;
using USStreetLookup = SmartyStreets.USStreetApi.Lookup;
using Marketplace.Common.Services.SmartyStreets.Mappers;
using System.Linq;
using Flurl;
using Flurl.Http;

namespace Marketplace.Common.Services
{
	public interface ISmartyStreetsService
	{
		Task<AddressValidation> ValidateAddress(Address address);
		Task<AddressValidation> ValidateAddress(BuyerAddress address);
	}

	public class SmartyStreetsService : ISmartyStreetsService
	{
		private readonly AppSettings _settings;
		private readonly ClientBuilder _builder;
		private readonly string AutoCompleteBaseUrl = "https://us-autocomplete.api.smartystreets.com";

		public SmartyStreetsService(AppSettings settings)
		{
			_settings = settings;
			_builder = new ClientBuilder(_settings.SmartyStreetSettings.AuthID, _settings.SmartyStreetSettings.AuthToken);
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
			var candidate = await ValidateSingleUSAddress(address);
			if (candidate.Count == 0)
			{
				var suggestions = await USAutoComplete(address);
				response.AreSuggestionsValid = false;
				response.SuggestedAddresses = SmartyStreetMappers.Map(suggestions);
			}
			else if (CandidateModified(candidate[0]))
			{
				response.SuggestedAddresses = new List<Address> { SmartyStreetMappers.Map(candidate[0]) };
			} else
			{
				response.IsRawAddressValid = true;
			}
			return response;
		}

		private async Task<List<Candidate>> ValidateSingleUSAddress(Address address)
		{
			var client = _builder.BuildUsStreetApiClient();
			var lookup = SmartyStreetMappers.MapToUSStreet(address);
			client.Send(lookup);
			return await Task.FromResult(lookup.Result);
		}

		private async Task<AutoCompleteResponse> USAutoComplete(Address address)
		{
			var suggestions = await AutoCompleteBaseUrl
				.AppendPathSegment("suggest")
				.SetQueryParam("auth-id", _settings.SmartyStreetSettings.AuthID)
				.SetQueryParam("auth-token", _settings.SmartyStreetSettings.AuthToken)
				.SetQueryParam("prefix", $"{address.Street1} {address.Street2}")
				.SetQueryParam("geolocate", "null")
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
