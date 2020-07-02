using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ordercloud.integrations.library;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ordercloud.integrations.cms.Models
{
	public class Document
	{
		[JsonProperty("ID"), CosmosInteropID, Required]
		public string InteropID { get; set; }
		[ApiIgnore]
		public string SchemaID { get; set; }
		[ApiReadOnly]
		public string SchemaSpecUrl { get; set; }
		[ApiIgnore]
		public string SellerOrgID { get; set; } // This field only needs to be public until it can be read from a token or /me 
		[Required]
		public JObject Doc { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/InteropID", "/SchemaID" }} // Should interopID be unique across schemas?
			};
		}
	}
}
