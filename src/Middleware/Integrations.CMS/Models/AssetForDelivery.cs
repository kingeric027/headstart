using Marketplace.CMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Integrations.CMS.Models
{
	public class AssetForDelivery : Asset
	{
		public int ListOrder { get; set; }

		public AssetForDelivery(Asset asset, int listOrder)
		{
			ListOrder = listOrder;
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
