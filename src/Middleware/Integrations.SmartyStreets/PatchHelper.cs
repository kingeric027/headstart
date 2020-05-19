using System;
using System.Collections.Generic;
using System.Text;

namespace Integrations.SmartyStreets
{
	public static class PatchHelper
	{
		public static T PatchObject<T>(T patch, T existing)
		{
			// todo: add test for this function 
			var patchType = patch.GetType();
			var propertiesInPatch = patchType.GetProperties();
			foreach (var property in propertiesInPatch)
			{
				var patchValue = property.GetValue(patch);
				if (patchValue != null)
				{
					property.SetValue(existing, patchValue, null);
				}
			}
			return existing;
		}
	}
}
