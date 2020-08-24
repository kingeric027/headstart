using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.cms
{
	public static class ListArgsExtensions
	{
		public static ListArgs<K> MapTo<T, K>(this ListArgs<T> args, ListArgMap propertyMap)
		{
			return new ListArgs<K>()
			{
				Page = args.Page,
				PageSize = args.PageSize,
				Search = args.Search,
				SearchOn = args.SearchOn == null ? null : propertyMap[args.SearchOn],
				SortBy = args.SortBy.Select(s =>
				{
					var property = s.TrimStart('!');
					s.Replace(property, propertyMap[property]);
					return s;
				}).ToList(),
				Filters = args.Filters.Select(f =>
				{
					f.Name = propertyMap[f.Name];
					f.QueryParams = f.QueryParams.Select(qp => new Tuple<string, string>(propertyMap[qp.Item1], qp.Item2)).ToList();
					return f;
				}).ToList()
			};
		}
	}

	public class ListArgMap : Dictionary<string, string>
	{
		public new string this[string key]
		{
			get {
				TryGetValue(key, out var value);
				return value ?? key;  // If the dictionary doesn't contain that key, return the key itself as the value 
			}
		}
	}
}
