using System;
using System.Collections.Generic;
using System.Text;

namespace ordercloud.integrations.cms
{
	public class CMSConfig
	{
		public string BaseUrl { get; set; }
		public string BlobStorageHostUrl { get; set; }
		public string BlobStorageConnectionString { get; set; }
	}
}
