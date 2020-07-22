using ordercloud.integrations.library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ordercloud.integrations.cms.Mappers
{
	public static class ListArgsExtensions
	{
		public static ListArgs<K> MapTo<T, K>(this ListArgs<T> args, Dictionary<string, string> propertyMap)
		{
			try
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
			} catch (KeyNotFoundException e)
			{
				throw new InvalidPropertyException(typeof(T).Name, e.Message); 
			}
			catch (Exception e) { throw e; }
		}
	}
}
