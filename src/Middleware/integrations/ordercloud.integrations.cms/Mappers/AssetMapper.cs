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
		private static readonly string[] ValidImageFormats = new[] { "image/png", "image/jpg", "image/jpeg" };

		public static (AssetDO, IFormFile) MapFromUpload(CMSConfig config, AssetContainerDO container, AssetUpload form)
		{
			if (!(form.File == null ^ form.Url == null))
			{
				throw new AssetUploadValidationException("Asset upload must include either File or Url override but not both.");
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
			TypeSpecificMapping(ref asset, form);
			return (asset, form.File);
		}

		private static List<string> MapTags(string tags)
		{
			return tags == null ? new List<string>() : tags.Split(",").Select(t => t.Trim()).Where(t => t != "").ToList();
		}

		private static void TypeSpecificMapping(ref AssetDO asset, AssetUpload form)
		{
			switch (asset.Type)
			{
				case AssetType.Image:
					ImageSpecificMapping(ref asset, form);
					return;
				case AssetType.Attachment:
				case AssetType.Structured:
				case AssetType.Theme:
				default:
					return;
			}
		}

		private static void ImageSpecificMapping(ref AssetDO asset, AssetUpload form)
		{
			if (form.File == null) return;
			if (!ValidImageFormats.Contains(form.File.ContentType)) 
			{
				throw new AssetUploadValidationException($"Image Uploads must be one of these file types - {string.Join(", ", ValidImageFormats)}");
			}
			using (var image = Image.FromStream(form.File.OpenReadStream()))
			{
				asset.Metadata.ImageWidth = image.Width;
				asset.Metadata.ImageHeight = image.Height;
				asset.Metadata.ImageHorizontalResolution = (decimal) image.HorizontalResolution;
				asset.Metadata.ImageVerticalResolution = (decimal) image.VerticalResolution;

				// TODO - potentially image resizing?
			}
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
