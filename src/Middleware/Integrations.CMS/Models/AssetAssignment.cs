﻿using Cosmonaut.Attributes;
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
		[ApiIgnore, CosmosPartitionKey]
		public string ContainerID { get; set; }
		[Required]
		public ResourceType ResourceType { get; set; }
		[Required]
		public string ResourceID { get; set; }
		public string ResourceParentID { get; set; }
		[Required]
		public string AssetID { get; set; }
		public int AssetListOrder { get; set; } // Within the context of a single oc resource 
		[ApiReadOnly]
		public Asset Asset { get; set; }

		public new static Collection<UniqueKey> GetUniqueKeys()
		{
			return new Collection<UniqueKey>
			{
				new UniqueKey() { Paths = new Collection<string> { "/AssetID", "/ResourceID", "/ResourceType", "/ResourceParentID" }}
			};
		}
	}
}
