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
			};
			var candidate = await ValidateSingleUSAddress(address);
			if (candidate.Count == 0)
			{
				var suggestions = await USAutoComplete(address);
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
			var lookup = SmartyStreetMappers.Map(address);
			client.Send(lookup);
			return await Task.FromResult(lookup.Result);
		}

		private async Task<Suggestion[]> USAutoComplete(Address address)
		{
			var client = _builder.BuildUsAutocompleteApiClient();
			var lookup = new AutoCompleteLookup(SmartyStreetMappers.ToPlainText(address));
			client.Send(lookup);
			return await Task.FromResult(lookup.Result);
		}

		private bool CandidateModified(Candidate candidate) 
		{
			return true;
		}
	}
}
