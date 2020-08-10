using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;

namespace ordercloud.integrations.cms
{
	public static class AssetMapper
	{
		public static AssetDO MapFromUpload(CMSConfig config, AssetContainerDO container, AssetUpload form)
		{
			if (!(form.File == null ^ form.Url == null))
			{
				throw new AssetCreateValidationException("Asset create must include either File or Url override but not both.");
			}
			var asset = new AssetDO()
			{
				InteropID = form.ID ?? CosmosInteropID.New(),
				ContainerID = container.id,
				Url = form.Url,
				Title = form.Title,
				Tags = MapTags(form.Tags),
				Type = form.Type,
				FileName = form.FileName ?? form.File?.FileName,
				Metadata = new AssetMetadata()
				{
					ContentType = form.File?.ContentType,
					SizeBytes = (int?)form.File?.Length,
					IsUrlOverridden = form.Url != null
				}
			};
			asset.Url = asset.Url ?? $"{config.BlobStorageHostUrl}/assets-{container.id}/{asset.id}";
			return asset;
		}

		private static List<string> MapTags(string tags)
		{
			return tags == null ? new List<string>() : tags.Split(",").Select(t => t.Trim()).Where(t => t != "").ToList();
		}

		public static Asset MapTo(AssetDO asset)
		{
			return new Asset()
			{
				ID = asset.InteropID,
				Title = asset.Title,
				Active = asset.Active,
				Url = asset.Url,
				Type = asset.Type,
				Tags = asset.Tags,
				FileName = asset.FileName,
				Metadata = asset.Metadata,
				History = asset.History
			};
		}

		public static IEnumerable<Asset> MapTo(IEnumerable<AssetDO> assets)
		{
			return assets.Select(asset => MapTo(asset));
		}

		public static ListPage<Asset> MapTo(ListPage<AssetDO> listPage)
		{
			return new ListPage<Asset>
			{
				Meta = listPage.Meta,
				Items = MapTo(listPage.Items).ToList()
			};
		}
	}
}
