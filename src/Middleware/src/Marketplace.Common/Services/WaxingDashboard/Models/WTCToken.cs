﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Headstart.Common.Services.WaxingDashboard.Models
{
	public class WTCToken
	{		
		public string access_token { get; set; }
		public string token_type { get; set; }
		public int expires_in { get; set; }
		public string scope { get; set; }
	}
}