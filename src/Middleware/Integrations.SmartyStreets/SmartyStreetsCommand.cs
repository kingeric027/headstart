using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Integrations.SmartyStreets;
using Marketplace.Common.Services.SmartyStreets.Mappers;
using Marketplace.Models.Misc;
using Newtonsoft.Json;
using OrderCloud.SDK;
using SmartyStreets;
using SmartyStreets.USStreetApi;

namespace Marketplace.Common.Services.SmartyStreets
{
	public interface ISmartyStreetsCommand
	{
		Task<AddressValidation<Address>> ValidateAddress(Address address);
		Task<AddressValidation<BuyerAddress>> ValidateAddress(BuyerAddress address);
	}

	public class SmartyStreetsCommand : ISmartyStreetsCommand
	{
		private readonly SmartyStreetsConfig _config;
		private readonly ClientBuilder _builder;
		private readonly string AutoCompleteBaseUrl = "https://us-autocomplete-pro.api.smartystreets.com";

		public SmartyStreetsCommand(SmartyStreetsConfig config)
		{
			_config = config;
			_builder = new ClientBuilder(_config.AuthID, _config.AuthToken);
		}

		public async Task<AddressValidation<Address>> ValidateAddress(Address address)
		{
			var response = new AddressValidation<Address>(address);
			var lookup = AddressMapper.MapToUSStreetLookup(address);
			var candidate = await ValidateSingleUSAddress(lookup); // Always seems to return 1 or 0 candidates
			if (candidate.Count > 0)
			{
				response.ValidAddress = AddressMapper.Map(candidate[0], address);
				response.GapBeteenRawAndValid = candidate[0].Analysis.DpvFootnotes;
			}
			else
			{
				// no valid address found
				var suggestions = await USAutoCompletePro($"{address.Street1} {address.Street2}");
				response.SuggestedAddresses = AddressMapper.Map(suggestions, address);
			}
			return response;
		}

		public async Task<AddressValidation<BuyerAddress>> ValidateAddress(BuyerAddress address)
		{
			var response = new AddressValidation<BuyerAddress>(address);
			var lookup = BuyerAddressMapper.MapToUSStreetLookup(address);
			var candidate = await ValidateSingleUSAddress(lookup); // Always seems to return 1 or 0 candidates
			if (candidate.Count > 0)
			{
				response.ValidAddress = BuyerAddressMapper.Map(candidate[0], address);
				response.GapBeteenRawAndValid = candidate[0].Analysis.DpvFootnotes;
			}
			else
			{
				// no valid address found
				var suggestions = await USAutoCompletePro($"{address.Street1} {address.Street2}");
				response.SuggestedAddresses = BuyerAddressMapper.Map(suggestions, address);
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
				.SetQueryParam("key", _config.WebsiteKey)
				.SetQueryParam("search", search)
				.WithHeader("Referer", _config.RefererHost)
				.GetJsonAsync<AutoCompleteResponse>();

			return suggestions;
		}
	}
}
