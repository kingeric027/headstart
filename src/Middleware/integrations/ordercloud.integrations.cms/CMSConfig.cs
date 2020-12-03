using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public class CMSConfig
	{
		public string BaseUrl { get; set; } // only used for schema links
		public string BlobStorageHostUrl { get; set; } // only used for placeholder images
	}
}
