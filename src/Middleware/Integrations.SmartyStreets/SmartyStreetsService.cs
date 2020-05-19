using Flurl;
using Flurl.Http;
using SmartyStreets;
using SmartyStreets.USStreetApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Integrations.SmartyStreets
{
	public interface ISmartyStreetsService {
		// returns many incomplete address suggestions
		Task<AutoCompleteResponse> USAutoCompletePro(string search);
		// returns one or zero very complete suggestions
		Task<List<Candidate>> ValidateSingleUSAddress(Lookup lookup);
	}
	public class SmartyStreetsService : ISmartyStreetsService
	{
		private readonly SmartyStreetsConfig _config;
		private readonly ClientBuilder _builder;
		private readonly string AutoCompleteBaseUrl = "https://us-autocomplete-pro.api.smartystreets.com";

		public SmartyStreetsService(SmartyStreetsConfig config)
		{
			_config = config;
			_builder = new ClientBuilder(_config.AuthID, _config.AuthToken);
		}

		public async Task<List<Candidate>> ValidateSingleUSAddress(Lookup lookup)
		{
			var client = _builder.BuildUsStreetApiClient();
			client.Send(lookup);
			return await Task.FromResult(lookup.Result);
		}

		public async Task<AutoCompleteResponse> USAutoCompletePro(string search)
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
