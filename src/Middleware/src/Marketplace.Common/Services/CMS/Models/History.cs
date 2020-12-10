﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Services.CMS.Models
{
	public class History
	{
		public DateTimeOffset DateCreated { get; set; }
		public string CreatedByUserID { get; set; }
		public DateTimeOffset DateUpdated { get; set; }
		public string UpdatedByUserID { get; set; }
	}
}