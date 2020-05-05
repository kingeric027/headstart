using Cosmonaut.Attributes;
using Integrations.CMS.Models;
using Marketplace.Helpers.Attributes;
using Marketplace.Helpers.Helpers.Attributes;
using Marketplace.Helpers.Models;
using Microsoft.Azure.Documents;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Marketplace.CMS.Models
{
	[SwaggerModel]
	[CosmosCollection("assetassignments")]
	public class AssetAssignment : CosmosObject
	{
		[Required]
		public ResourceType ResourceType { get; set; }
		[Required, CosmosPartitionKey]
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; }
		public List<string> ImageAssetIDs { get; set; }
		public List<string> OtherAssetIDs { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/ResourceID", "/ResourceType", "/ResourceParentID" }}
			};
		}
	}
}
