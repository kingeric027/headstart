using System;
using System.Collections.Generic;
using System.Text;
using SmartyStreets.USAutocompleteApi;


namespace Marketplace.Common.Services.SmartyStreets.Mappers
{
	public class AutoCompleteResponse
	{
		public List<Suggestion> suggestions { get; set; }
	}
}
