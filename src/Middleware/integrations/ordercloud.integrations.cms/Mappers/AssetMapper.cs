using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ordercloud.integrations.library.Cosmos;
using OrderCloud.SDK;
using ordercloud.integrations.library;

namespace ordercloud.integrations.cms
{
	public static class AssetMapper
	{
		public static AssetDO MapFromUpload(AssetContainerDO container, AssetUpload form)
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
				Type = DetectAssetType(form.File),
				Active = form.Active,
				FileName = form.FileName ?? form.File?.FileName,
				Metadata = new AssetMetadata()
				{
					ContentType = form.File?.ContentType,
					SizeBytes = (int?)form.File?.Length,
					IsUrlOverridden = form.Url != null
				}
			};
			return asset;
		}

		private static List<string> MapTags(string tags)
		{
			return tags == null ? new List<string>() : tags.Split(",").Select(t => t.Trim()).Where(t => t != "").ToList();
		}

		public static Asset MapTo(CMSConfig config, AssetDO asset)
		{
			return new Asset()
			{
				ID = asset.InteropID,
				Title = asset.Title,
				Active = asset.Active,
				Url = asset.Url ?? $"{config.BlobStorageHostUrl}/assets-{asset.ContainerID}/{asset.id}",
				Type = asset.Type,
				Tags = asset.Tags,
				FileName = asset.FileName,
				Metadata = asset.Metadata,
				History = asset.History
			};
		}

		public static IEnumerable<Asset> MapTo(CMSConfig config, IEnumerable<AssetDO> assets)
		{
			return assets.Select(asset => MapTo(config, asset));
		}

		public static ListPage<Asset> MapTo(CMSConfig config, ListPage<AssetDO> listPage)
		{
			return new ListPage<Asset>
			{
				Meta = listPage.Meta,
				Items = MapTo(config, listPage.Items).ToList()
			};
		}

		public static ListArgs<AssetDO> MapTo(this ListArgs<Asset> args)
		{
			return args.MapTo<Asset, AssetDO>(new ListArgMap()
			{
				{"ID", "InteropID" }
			});
		}

		private static bool ContainsOneOf(this string stringToTest, params string[] words) => words.Any(stringToTest.Contains);
		private static bool EqualsOneOf(this string stringToTest, params string[] words) => words.Any(stringToTest.Equals);

		public static AssetType DetectAssetType(IFormFile file)
		{
			var mimeAssetType = DetectAssetTypeFromContentType(file.ContentType); // First try content type. Its more reliable. 
			if (mimeAssetType == AssetType.Unknown)
			{
				return DetectAssetTypeFromFileName(file.FileName); // Checking the file name is a backup.
			}
			return mimeAssetType;
		}

		// https://www.iana.org/assignments/media-types/media-types.xhtml
		// This standard is very detailed, but not a perfect representation of reality. For exameple, it seems to be missing image/jpeg.
		public static AssetType DetectAssetTypeFromContentType(string mimeType)
		{
			if (mimeType == null || mimeType == "" || mimeType == "application/octet-stream") return AssetType.Unknown;
			if (mimeType.StartsWith("image/")) return AssetType.Image;
			if (mimeType.StartsWith("audio/")) return AssetType.Audio;
			if (mimeType.StartsWith("video/")) return AssetType.Video;

			if (mimeType.Contains("pdf")) return AssetType.PDF;
			if (mimeType.ContainsOneOf("presentation", "powerpoint", "slide", "keynote", "impress")) return AssetType.Presentation;
			if (mimeType.ContainsOneOf("spread", "excel", "xls", "csv", "calc", "numbers")) return AssetType.SpreadSheet;
			if (mimeType.ContainsOneOf("zip", "tar", "zlib", "zstd", "compressed", "7z")) return AssetType.Compressed;
			if (mimeType.ContainsOneOf("html", "xml", "markdown", "yaml")) return AssetType.Markup;
			if (mimeType.ContainsOneOf("video", "mplayer", "quicktime", "movie")) return AssetType.Video;
			if (mimeType.ContainsOneOf("music")) return AssetType.Audio;

			if (mimeType.ContainsOneOf("javascript")) return AssetType.Code;
			if (mimeType.Contains("json")) return AssetType.JSON;

			if (mimeType.EqualsOneOf("text/css")) return AssetType.Code;

			if (mimeType.Contains("text/")) return AssetType.Text;
			return AssetType.Unknown;
		}

		public static AssetType DetectAssetTypeFromFileName(string fileName)
		{
			if (fileName == null) return AssetType.Unknown;

			var fileExtension = fileName.Split('.').Last();

			if (fileExtension == "") return AssetType.Unknown;
			if (fileExtension.EqualsOneOf("jpg", "jpeg", "png", "svg", "gif", "ico", "bmp", "bpm", "tif", "tiff", "bpg", "psd")) return AssetType.Image;
			if (fileExtension.EqualsOneOf("mp3", "m4a", "flac", "wav", "wma", "aac", "pcm", "aiff")) return AssetType.Audio;
			if (fileExtension.EqualsOneOf("mp4", "mov", "wmv", "flv", "avi", "webm", "mkv")) return AssetType.Video;
			if (fileExtension.EqualsOneOf("csv", "ods", "numbers", "sxc", "xl")) return AssetType.SpreadSheet;
			if (fileExtension.EqualsOneOf("zip", "tbz", "pkg", "7z", "arj", "rar", "gz")) return AssetType.Compressed;
			if (fileExtension.EqualsOneOf("html", "xml", "md", "yaml", "yml")) return AssetType.Markup;
			if (fileExtension.EqualsOneOf("css", "js", "cs", "php", "py", "java", "sh", "c", "vb")) return AssetType.Code;
			if (fileExtension.EqualsOneOf("txt", "doc", "docx", "odt", "wpd", "rtf")) return AssetType.Text;

			if (fileExtension.Equals("pdf")) return AssetType.PDF;
			if (fileExtension.Equals("json")) return AssetType.JSON;
			if (fileExtension.EqualsOneOf("key", "odp")) return AssetType.Presentation;

			if (fileExtension.Contains("tar")) return AssetType.Compressed;
			if (fileExtension.Contains("html")) return AssetType.Markup;
			if (fileExtension.Contains("pp")) return AssetType.Presentation;
			if (fileExtension.Contains("xls")) return AssetType.SpreadSheet;

			return AssetType.Unknown; 
		}
	}
}
