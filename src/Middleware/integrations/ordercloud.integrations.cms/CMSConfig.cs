using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public class CMSConfig
	{
		public string BaseUrl { get; set; }
		public string BlobStorageHostUrl { get; set; } // only used for "placeholder" images
		public string BlobStorageConnectionString { get; set; } // not used in the CMS anymore
	}
}
