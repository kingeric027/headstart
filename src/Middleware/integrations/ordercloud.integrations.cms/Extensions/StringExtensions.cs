using System.Linq;

namespace ordercloud.integrations.cms
{

	public static class StringExtensions
	{
		// https://www.iana.org/assignments/media-types/media-types.xhtml
		// This standard is very detailed, but not a perfect representation of reality. For exameple, it seems to be missing image/jpeg.
		public static AssetType ConvertFromContentType(this string mimeType)
		{
			if (mimeType == null) return AssetType.Unknown;
			if (mimeType.StartsWith("image/")) return AssetType.Image;
			if (mimeType.StartsWith("audio/")) return AssetType.Audio;
			if (mimeType.StartsWith("video/")) return AssetType.Video;

			if (mimeType.Contains("pdf")) return AssetType.PDF;
			if (mimeType.ContainsOneOf("presentation", "powerpoint", "slide")) return AssetType.Slides;
			if (mimeType.ContainsOneOf("spread", "excel", "xls", "csv")) return AssetType.SpreadSheet;
			if (mimeType.ContainsOneOf("zip", "tar", "zlib", "zstd", "compressed", "7z")) return AssetType.Compressed;
			if (mimeType.ContainsOneOf("html", "xml", "markdown", "yaml")) return AssetType.Markup;
			if (mimeType.ContainsOneOf("javascript")) return AssetType.Code;
			if (mimeType.Contains("json")) return AssetType.JSON;

			if (mimeType.EqualsOneOf("text/css")) return AssetType.Code;

			if (mimeType.Contains("text/")) return AssetType.Text;
			return AssetType.Unknown;
		}

		private static bool ContainsOneOf(this string stringToTest, params string[] words) => words.Any(stringToTest.Contains);
		private static bool EqualsOneOf(this string stringToTest, params string[] words) => words.Any(stringToTest.Equals);

	}
}
