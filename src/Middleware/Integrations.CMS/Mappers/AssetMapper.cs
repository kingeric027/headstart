using Integrations.CMS;
using Integrations.CMS.Models;
using Marketplace.CMS.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Marketplace.CMS.Mappers
{
	public static class AssetMapper
	{
		private static readonly string[] ValidImageFormats = new[] { "image/png", "image/jpg", "image/jpeg" };

		public static (Asset, IFormFile) MapFromUpload(CMSConfig config, AssetContainer container, AssetUpload form)
		{
			if (!(form.File == null ^ form.Url == null))
			{
				throw new AssetUploadValidationException("Asset upload must include either File or Url override but not both.");
			}
			var asset = new Asset()
			{
				InteropID = form.ID,
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

		private static void TypeSpecificMapping(ref Asset asset, AssetUpload form)
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

		private static void ImageSpecificMapping(ref Asset asset, AssetUpload form)
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
	}
}
