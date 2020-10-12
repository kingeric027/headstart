using NUnit.Framework;
using ordercloud.integrations.cms;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Tests
{
	public class ContentTypeTests
	{
		[Test]
		[TestCase(null, AssetType.Unknown)]
		[TestCase("", AssetType.Unknown)]
		[TestCase("sdfsdfsdfsfdsf", AssetType.Unknown)]
		[TestCase("application/octet-stream", AssetType.Unknown)]
		[TestCase("text/plain", AssetType.Text)]
		[TestCase("application/pdf", AssetType.PDF)]
		[TestCase("text/csv", AssetType.SpreadSheet)]
		[TestCase("text/css", AssetType.Code)]
		[TestCase("text/javascript", AssetType.Code)]
		[TestCase("application/javascript", AssetType.Code)]
		[TestCase("text/markdown", AssetType.Markup)]
		[TestCase("text/html", AssetType.Markup)]
		[TestCase("application/xml", AssetType.Markup)]
		[TestCase("text/xml", AssetType.Markup)]
		[TestCase("text/json", AssetType.JSON)]
		[TestCase("image/png", AssetType.Image)]
		[TestCase("image/jpeg", AssetType.Image)]		
		[TestCase("image/bmp", AssetType.Image)]	
		[TestCase("application/zip", AssetType.Compressed)]
		[TestCase("application/gzip", AssetType.Compressed)]
		[TestCase("application/x-tar", AssetType.Compressed)]
		[TestCase("application/zlib", AssetType.Compressed)]
		[TestCase("application/zstd", AssetType.Compressed)]
		[TestCase("application/x-7z-compressed", AssetType.Compressed)]
		[TestCase("application/vnd.oasis.opendocument.presentation", AssetType.Presentation)]
		[TestCase("application/vnd.ms-powerpoint", AssetType.Presentation)]
		[TestCase("application/vnd.apple.keynote", AssetType.Presentation)]
		[TestCase("application/vnd.openxmlformats-officedocument.presentationml.presentation", AssetType.Presentation)]
		[TestCase("application/vnd.apple.numbers", AssetType.SpreadSheet)]
		[TestCase("application/vnd.sun.xml.calc", AssetType.SpreadSheet)]
		[TestCase("application/vnd.ms-excel", AssetType.SpreadSheet)]	
		[TestCase("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", AssetType.SpreadSheet)]
		[TestCase("application/x-troff-msvideo", AssetType.Video)]
		public void converts_content_types_correctly(string contentType, AssetType expectedType)
		{
			var type = contentType.ConvertFromContentType();
			Assert.AreEqual(expectedType, type);
		}
	}
}
