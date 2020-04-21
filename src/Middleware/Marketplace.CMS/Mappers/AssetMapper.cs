using Marketplace.CMS.Models;
using Marketplace.Helpers;
using Marketplace.Helpers.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.CMS.Mappers
{
	public static class AssetMapper
	{
		private static readonly string[] ValidImageFormats = new [] { "image/png", "image/jpg", "image/jepg" };

		public static (Asset, IFormFile) Map(AssetContainer container, AssetUploadForm form)
		{
			var hasFile = form.File == null;
			var hasUrlOveride = form.UrlPathOveride == null;
			Require.That((hasFile ^ hasUrlOveride), new ErrorCode("Asset Upload", 400, "Asset upload must include either File or UrlPathOveride but not both."));
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
			return (asset, form.File);
		}
	}
}
