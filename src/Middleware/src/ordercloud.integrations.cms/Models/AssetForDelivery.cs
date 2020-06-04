using System;
using System.Collections.Generic;
using System.Text;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	[SwaggerModel]
	public class AssetForDelivery : Asset
	{
		public int ListOrderWithinType { get; set; }

		public AssetForDelivery() { }

		public AssetForDelivery(Asset asset, int listOrder)
		{
			ListOrderWithinType = listOrder;
			InteropID = asset.InteropID;
			Title = asset.Title;
			Active = asset.Active;
			Url = asset.Url;
			Type = asset.Type;
			Tags = asset.Tags;
			FileName = asset.FileName;
			Metadata = asset.Metadata;
		}
	}
}
