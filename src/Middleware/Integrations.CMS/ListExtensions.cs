using System;
using System.Collections.Generic;
using System.Text;

namespace Integrations.CMS
{
	public static class ListExtensions
	{
		public static void UniqueAdd(this List<string> list, string item)
		{ 
			if (list.Contains(item)) list.Add(item);
		}

		public static void MoveTo(this List<string> list, string item, int index)
		{
			list.Remove(item);
			index = Math.Min(index, list.Count);
			list.Insert(index, item);
		}
	}
}
