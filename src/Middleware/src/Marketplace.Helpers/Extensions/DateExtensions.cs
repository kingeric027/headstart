using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Helpers.Extensions
{
	public static class DateExtensions
	{
		public static DateTime UnixToDateTime(this string unix)
		{
			var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return dtDateTime.AddSeconds(int.Parse(unix)).ToLocalTime();
		}
	}
}
