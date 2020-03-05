using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Common.Extensions
{
	// todo: move to helpers lib
	public static class XpExtensions
	{
		public static void SetProperty(dynamic xp, string property, object value)
		{
			var x = (IDictionary<string, object>)xp;
			x[property] = value;
		}
	}
}
