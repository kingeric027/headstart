using Marketplace.CMS.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
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
		private static readonly string[] ValidImageFormats = new[] { "image/png", "image/jpg", "image/jepg" };

		public static (Asset, IFormFile) Map(AssetContainer container, AssetUploadForm form)
		{
			var hasFile = form.File != null;
			var hasUrlOveride = form.UrlPathOveride != null;
			if (!(hasFile ^ hasUrlOveride))
			{
				throw new AssetUploadValidationException("Asset upload must include either File or UrlPathOveride but not both.");
			}
			var asset = new Asset()
			{
				InteropID = form.ID,
				ContainerID = container.id,
				UrlPathOveride = form.UrlPathOveride,
				Title = form.Title,
				Tags = form.Tags,
				Type = form.Type,
				FileName = form.FileName ?? form.File?.FileName,
				Metadata = new AssetMetadata()
				{
					ContentType = form.File?.ContentType,
					SizeBytes = form.File?.Length,
					IsUrlOverridden = hasUrlOveride,
				}
			};
			TypeSpecificMapping(ref asset, form);
			return (asset, form.File);
		}

		private static void TypeSpecificMapping(ref Asset asset, AssetUploadForm form)
		{
			switch (asset.Type)
			{
				case AssetType.Image:
					ImageSpecificMapping(ref asset, form);
					return;
				case AssetType.StructuredData:
				case AssetType.Attachment:
				case AssetType.Theme:
					return;
			}
		}

		private static void ImageSpecificMapping(ref Asset asset, AssetUploadForm form)
		{
			if (!ValidImageFormats.Contains(form.File.ContentType)) 
			{
				throw new AssetUploadValidationException($"Image Uploads must be one of these file types - {string.Join(", ", ValidImageFormats)}");
			}
			using (var image = Image.FromStream(form.File.OpenReadStream()))
			{
				asset.Metadata.ImageWidth = image.Width;
				asset.Metadata.ImageWidth = image.Width;
				// TODO - this is where we could potentially do image resizing in the future?
			}
		}
	}
}
