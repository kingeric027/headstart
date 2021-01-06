﻿using OrderCloud.SDK;

namespace Marketplace.Models.Misc
{
	public class HSSecurityProfile
	{
		public CustomRole CustomRole { get; set; }
		public ApiRole[] Roles { get; set; }
		public CustomRole[] CustomRoles { get; set; }
	}
}
