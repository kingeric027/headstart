using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public class Resource
	{
		public Resource(ResourceType type, string id, string parentID = null)
		{
			Type = type;
			ID = id;
			ParentID = parentID;
		}
		public string ID { get; set; }
		public string ParentID { get; set; } = null;
		public ResourceType Type { get; set; }
	}

	public enum ResourceType
	{
		Products, Categories, Catalogs, Promotions, Suppliers, Buyers, ProductFacets
	}
}
